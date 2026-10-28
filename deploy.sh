
#!/usr/bin/env bash
set -Eeuo pipefail

# Локальная публикация и архив
PROJECT_DIR="$HOME/projects/level-english-backup/admin-blazor"
ARCHIVE="$PROJECT_DIR/admin-blazor-linux.tar.gz"

# Ubuntu — существующий рабочий сервер
REMOTE_USER="ubuntu"
REMOTE_HOST="77.240.38.4"
REMOTE_ARCHIVE="/home/ubuntu/admin-blazor-linux.tar.gz"

# Рабочий каталог приложения — НЕ меняем
APP_DIR="/opt/level-english-admin"
SERVICE="level-english-blazor.service"

if [[ ! -f "$ARCHIVE" ]]; then
    echo "ОШИБКА: архив не найден: $ARCHIVE"
    exit 1
fi

echo "==> Проверяю архив..."
if ! tar -tzf "$ARCHIVE" >/dev/null; then
    echo "ОШИБКА: архив повреждён."
    exit 1
fi

echo "==> Передаю архив на Ubuntu..."
scp "$ARCHIVE" "${REMOTE_USER}@${REMOTE_HOST}:${REMOTE_ARCHIVE}"

echo "==> Обновляю приложение в существующем каталоге..."
ssh -t "${REMOTE_USER}@${REMOTE_HOST}" \
    "sudo bash -s -- '$REMOTE_ARCHIVE' '$APP_DIR' '$SERVICE'" <<'REMOTE_SCRIPT'
set -Eeuo pipefail

ARCHIVE="$1"
APP_DIR="$2"
SERVICE="$3"
STAMP="$(date +%Y%m%d-%H%M%S)"
BACKUP="/opt/level-english-admin-backup-$STAMP"

if [[ ! -d "$APP_DIR" ]]; then
    echo "ОШИБКА: рабочий каталог не найден: $APP_DIR"
    exit 1
fi

if [[ ! -f "$ARCHIVE" ]]; then
    echo "ОШИБКА: архив не найден: $ARCHIVE"
    exit 1
fi

echo "==> Создаю резервную копию текущего приложения..."
cp -a "$APP_DIR" "$BACKUP"
echo "Резервная копия: $BACKUP"

echo "==> Сохраняю конфигурацию..."
CONFIG_BACKUP="$(mktemp -d)"
if [[ -f "$APP_DIR/appsettings.json" ]]; then
    cp -a "$APP_DIR/appsettings.json" "$CONFIG_BACKUP/"
fi
if [[ -f "$APP_DIR/appsettings.Production.json" ]]; then
    cp -a "$APP_DIR/appsettings.Production.json" "$CONFIG_BACKUP/"
fi

echo "==> Проверяю архив на сервере..."
tar -tzf "$ARCHIVE" >/dev/null

echo "==> Останавливаю службу..."
systemctl stop "$SERVICE"

echo "==> Распаковываю обновление в существующий каталог..."
tar -xzf "$ARCHIVE" -C "$APP_DIR"

echo "==> Восстанавливаю конфигурацию..."
if [[ -f "$CONFIG_BACKUP/appsettings.json" ]]; then
    cp -a "$CONFIG_BACKUP/appsettings.json" "$APP_DIR/"
fi
if [[ -f "$CONFIG_BACKUP/appsettings.Production.json" ]]; then
    cp -a "$CONFIG_BACKUP/appsettings.Production.json" "$APP_DIR/"
fi
rm -rf "$CONFIG_BACKUP"

echo "==> Запускаю службу..."
systemctl start "$SERVICE"
sleep 2

if systemctl is-active --quiet "$SERVICE"; then
    echo "УСПЕХ: служба запущена."
    systemctl --no-pager --full status "$SERVICE"
    echo "Резервная копия: $BACKUP"
else
    echo "ОШИБКА: служба не запустилась."
    journalctl -u "$SERVICE" -n 60 --no-pager
    echo "Предыдущая версия сохранена в: $BACKUP"
    exit 1
fi

rm -f "$ARCHIVE"
REMOTE_SCRIPT

echo
echo "==> Скрипт переноса завершён."
echo "Проверь сайт в браузере."

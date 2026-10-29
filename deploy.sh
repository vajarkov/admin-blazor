
#!/usr/bin/env bash
set -Eeuo pipefail

# Локальный проект и публикация
PROJECT_DIR="$HOME/projects/level-english-backup/admin-blazor"
PUBLISH_DIR="$PROJECT_DIR/publish-linux"
ARCHIVE="$PROJECT_DIR/admin-blazor-linux.tar.gz"

# Ubuntu — существующий рабочий сервер
REMOTE_USER="ubuntu"
REMOTE_HOST="77.240.38.4"
REMOTE_ARCHIVE="/home/ubuntu/admin-blazor-linux.tar.gz"

# Удаляем предыдущую публикацию
echo "==> Удаляю предыдущую публикацию..."
rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR"

# Собираем self-contained публикацию
echo "==> Собираю self-contained публикацию для linux-x64..."
dotnet publish "$PROJECT_DIR/admin-blazor.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -o "$PUBLISH_DIR"

# Создаём архив из свежей публикации
echo "==> Создаю архив..."
tar -czf "$ARCHIVE" -C "$PUBLISH_DIR" .

# Проверяем архив
echo "==> Проверяю архив..."
if ! tar -tzf "$ARCHIVE" >/dev/null; then
    echo "ОШИБКА: архив повреждён."
    exit 1
fi

# Передаём архив на Ubuntu
echo "==> Передаю архив на Ubuntu..."
scp "$ARCHIVE" "${REMOTE_USER}@${REMOTE_HOST}:${REMOTE_ARCHIVE}"

echo
echo "==> Публикация собрана и архив передан на Ubuntu."
echo "==> Теперь запусти свой скрипт обновления на сервере."

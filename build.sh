#!/bin/bash

# HDEMS Docker 镜像构建脚本

IMAGE_NAME="hdems-app"
ARCHIVE_NAME="${IMAGE_NAME}.tar.gz"

echo "=========================================="
echo "开始构建 Docker 镜像"
echo "镜像名称: ${IMAGE_NAME}"
echo "=========================================="

# 构建 Docker 镜像
docker build -t "${IMAGE_NAME}" .

# 检查构建是否成功
if [ $? -eq 0 ]; then
    echo ""
    echo "构建成功，开始导出镜像..."
    docker save "${IMAGE_NAME}" | gzip > "${ARCHIVE_NAME}"

    if [ $? -eq 0 ]; then
        echo "=========================================="
        echo "镜像已保存到: ${ARCHIVE_NAME}"
        ls -lh "${ARCHIVE_NAME}"
        echo "=========================================="
    else
        echo "导出失败！"
        exit 1
    fi
else
    echo "构建失败！"
    exit 1
fi

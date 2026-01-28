#!/bin/bash

# HDEMS Docker 镜像构建脚本
# 使用方法: ./build.sh [镜像名称] [标签]

# 默认配置
FULL_IMAGE_NAME="hdems-app:dev"

echo "=========================================="
echo "开始构建 Docker 镜像"
echo "镜像名称: ${FULL_IMAGE_NAME}"
echo "=========================================="

# 清理旧的构建缓存（可选，取消注释以启用）
# docker builder prune -f

# 构建 Docker 镜像
docker build \
  -t "${FULL_IMAGE_NAME}" \
  -f Dockerfile \
  .

docker build -t hdems-app .
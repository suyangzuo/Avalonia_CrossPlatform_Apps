#!/bin/bash

# Avalonia 桌面应用发布脚本
# 发布单文件（不包含 .NET 运行时）

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 默认配置
CONFIGURATION="Release"
SELF_CONTAINED="false"
PUBLISH_SINGLE_FILE="true"
PROJECT_PATH="Avalonia_CrossPlatform_Apps.Desktop/Avalonia_CrossPlatform_Apps.Desktop.csproj"

# 显示帮助信息
show_help() {
    echo "用法: $0 [选项] [平台]"
    echo ""
    echo "选项:"
    echo "  -h, --help              显示此帮助信息"
    echo "  -c, --config CONFIG     配置 (Debug/Release)，默认: Release"
    echo "  -o, --output DIR        输出目录，默认: ./publish-[平台]"
    echo ""
    echo "平台:"
    echo "  linux-x64               Linux 64位 (默认)"
    echo "  win-x64                  Windows 64位"
    echo "  osx-x64                  macOS Intel 64位"
    echo ""
    echo "示例:"
    echo "  $0                      # 发布 Linux 版本"
    echo "  $0 win-x64              # 发布 Windows 版本"
    echo "  $0 -o ./release win-x64 # 发布到指定目录"
}

# 解析参数
RUNTIME_ID="linux-x64"
OUTPUT_DIR=""
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        -c|--config)
            CONFIGURATION="$2"
            shift 2
            ;;
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        linux-x64|win-x64|osx-x64)
            RUNTIME_ID="$1"
            shift
            ;;
        *)
            echo -e "${RED}错误: 未知参数: $1${NC}"
            show_help
            exit 1
            ;;
    esac
done

# 设置输出目录
if [ -z "$OUTPUT_DIR" ]; then
    OUTPUT_DIR="./publish-${RUNTIME_ID}"
fi

# 显示发布信息
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  Avalonia 桌面应用发布${NC}"
echo -e "${GREEN}========================================${NC}"
echo -e "项目: ${YELLOW}${PROJECT_PATH}${NC}"
echo -e "配置: ${YELLOW}${CONFIGURATION}${NC}"
echo -e "平台: ${YELLOW}${RUNTIME_ID}${NC}"
echo -e "输出: ${YELLOW}${OUTPUT_DIR}${NC}"
echo -e "单文件: ${YELLOW}${PUBLISH_SINGLE_FILE}${NC}"
echo -e "自包含: ${YELLOW}${SELF_CONTAINED}${NC} (不包含 .NET 运行时)"
echo ""

# 检查项目文件是否存在
if [ ! -f "$PROJECT_PATH" ]; then
    echo -e "${RED}错误: 找不到项目文件: ${PROJECT_PATH}${NC}"
    exit 1
fi

# 执行发布
echo -e "${GREEN}开始发布...${NC}"
dotnet publish "$PROJECT_PATH" \
    -c "$CONFIGURATION" \
    -r "$RUNTIME_ID" \
    -p:PublishSingleFile="$PUBLISH_SINGLE_FILE" \
    -p:SelfContained="$SELF_CONTAINED" \
    -o "$OUTPUT_DIR"

# 检查发布结果
if [ $? -eq 0 ]; then
    # 如果是 Linux 平台，创建 .desktop 文件并复制图标
    if [ "$RUNTIME_ID" = "linux-x64" ]; then
        DESKTOP_FILE="${OUTPUT_DIR}/时钟.desktop"
        ICON_SOURCE="Avalonia_CrossPlatform_Apps/Assets/Clock.png"
        ICON_TARGET="${OUTPUT_DIR}/Clock.png"
        
        # 复制图标到发布目录
        if [ -f "$ICON_SOURCE" ]; then
            cp "$ICON_SOURCE" "$ICON_TARGET"
            echo -e "${GREEN}已复制图标文件${NC}"
        fi
        
        # 创建 .desktop 文件
        ABS_OUTPUT_DIR="$(cd "$OUTPUT_DIR" && pwd)"
        cat > "$DESKTOP_FILE" <<EOF
[Desktop Entry]
Version=1.0
Type=Application
Name=时钟
Comment=时钟应用
Exec=${ABS_OUTPUT_DIR}/Avalonia_CrossPlatform_Apps.Desktop
Icon=${ABS_OUTPUT_DIR}/Clock.png
Terminal=false
Categories=Utility;Application;
StartupNotify=true
EOF
        chmod +x "$DESKTOP_FILE"
        echo -e "${GREEN}已创建 .desktop 文件${NC}"
    fi
    
    echo ""
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}发布成功！${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo -e "输出目录: ${YELLOW}${OUTPUT_DIR}${NC}"
    echo ""
    
    # 显示生成的文件
    echo -e "${GREEN}生成的文件:${NC}"
    if [ "$RUNTIME_ID" = "win-x64" ]; then
        EXE_FILE="${OUTPUT_DIR}/Avalonia_CrossPlatform_Apps.Desktop.exe"
        if [ -f "$EXE_FILE" ]; then
            SIZE=$(du -h "$EXE_FILE" | cut -f1)
            echo -e "  ${YELLOW}${EXE_FILE}${NC} (${SIZE})"
        fi
    else
        EXE_FILE="${OUTPUT_DIR}/Avalonia_CrossPlatform_Apps.Desktop"
        if [ -f "$EXE_FILE" ]; then
            SIZE=$(du -h "$EXE_FILE" | cut -f1)
            echo -e "  ${YELLOW}${EXE_FILE}${NC} (${SIZE})"
        fi
    fi
    
    # 显示原生库文件
    echo ""
    echo -e "${GREEN}原生库文件:${NC}"
    find "$OUTPUT_DIR" -name "*.so" -o -name "*.dylib" -o -name "*.dll" | grep -E "(HarfBuzz|Skia)" | while read -r file; do
        SIZE=$(du -h "$file" | cut -f1)
        echo -e "  ${YELLOW}$(basename "$file")${NC} (${SIZE})"
    done
    
    echo ""
    echo -e "${YELLOW}注意:${NC}"
    echo -e "  - 目标机器需要安装 .NET 8.0 运行时"
    echo -e "  - 所有文件（可执行文件和原生库）必须在同一目录"
    
    if [ "$RUNTIME_ID" = "win-x64" ]; then
        echo -e "  - 运行: ${YELLOW}${EXE_FILE}${NC}"
    else
        echo -e "  - 运行: ${YELLOW}./${EXE_FILE}${NC}"
    fi
else
    echo ""
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}发布失败！${NC}"
    echo -e "${RED}========================================${NC}"
    exit 1
fi


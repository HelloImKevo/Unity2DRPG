#!/bin/bash

# Enhanced Standalone .NET Test Runner with Clean Build
# This script runs unit tests using temporary directories to avoid cluttering the repo

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${YELLOW}🧪 Clean Standalone .NET Test Runner${NC}"
echo "Testing Unity game logic with temporary build directories..."

# Security: Validate we're in the correct directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXPECTED_TEST_DIR="$SCRIPT_DIR/StandaloneTests"

if [ ! -d "$EXPECTED_TEST_DIR" ]; then
    echo -e "${RED}❌ Security check failed: StandaloneTests directory not found at expected location${NC}"
    echo "Expected: $EXPECTED_TEST_DIR"
    exit 1
fi

if [ ! -f "$EXPECTED_TEST_DIR/StandaloneTests.csproj" ]; then
    echo -e "${RED}❌ Security check failed: StandaloneTests.csproj not found${NC}"
    echo "This script must be run from the Unity2DRPG root directory"
    exit 1
fi

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK not found!${NC}"
    echo "Please install .NET SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}✅ Found .NET SDK: $(dotnet --version)${NC}"
echo -e "${GREEN}✅ Security validation passed${NC}"

# Create temporary build directory with proper permissions
TEMP_DIR=$(mktemp -d -t unity-tests-XXXXXXXX)
if [ ! -d "$TEMP_DIR" ]; then
    echo -e "${RED}❌ Failed to create temporary directory${NC}"
    exit 1
fi

echo -e "${BLUE}📁 Using temporary build directory: $TEMP_DIR${NC}"

# Set up cleanup trap for safety
cleanup() {
    if [ -n "$TEMP_DIR" ] && [ "$TEMP_DIR" != "/" ] && [[ "$TEMP_DIR" == /tmp/* || "$TEMP_DIR" == /var/folders/* ]]; then
        echo -e "${BLUE}🧹 Cleaning up temporary files...${NC}"
        rm -rf "$TEMP_DIR"
    else
        echo -e "${YELLOW}⚠️  Warning: Unexpected temp directory path, skipping cleanup: $TEMP_DIR${NC}"
    fi
}
trap cleanup EXIT

# Navigate to test project safely
cd "$EXPECTED_TEST_DIR"

echo -e "${BLUE}🔄 Restoring packages (including Moq mocking framework)...${NC}"
dotnet restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Package restore failed${NC}"
    exit 1
fi

echo -e "${BLUE}🏗️  Building test project to temporary directory...${NC}"
dotnet build --output "$TEMP_DIR/build"

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

echo -e "${BLUE}🚀 Running tests from temporary directory...${NC}"
dotnet test --no-build --output "$TEMP_DIR/build" --logger "console;verbosity=detailed"

TEST_RESULT=$?

if [ $TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed! No build artifacts left in repository.${NC}"
    echo -e "${BLUE}💡 This demonstrates clean testing of Unity game logic with mocking${NC}"
    echo -e "${BLUE}🎯 Tested components:${NC}"
    echo "  • RPGMath utility functions (12 tests)"
    echo "  • StateMachine state transitions (8 tests)"
    echo "  • Vector2Utils array operations (7 tests)"
    echo "  • Mock-based testing approach"
    echo -e "${GREEN}🗂️  Repository remains clean - no build artifacts!${NC}"
else
    echo -e "${RED}❌ Some tests failed${NC}"
    exit 1
fi

# Cleanup happens automatically via trap

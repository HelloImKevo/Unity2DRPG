#!/bin/bash

# Enhanced Standalone .NET Test Runner with Mocking Framework
# This script runs unit tests using standard .NET tooling and Moq for mocking

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${YELLOW}🧪 Enhanced Standalone .NET Test Runner${NC}"
echo "Testing Unity game logic with Moq mocking framework..."

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

# Navigate to test project safely
cd "$EXPECTED_TEST_DIR"

echo -e "${BLUE}🔄 Restoring packages (including Moq mocking framework)...${NC}"
dotnet restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Package restore failed${NC}"
    exit 1
fi

echo -e "${BLUE}🏗️  Building test project...${NC}"
dotnet build

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

echo -e "${BLUE}🚀 Running tests with mocking support...${NC}"
dotnet test --logger "console;verbosity=detailed"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
    echo -e "${BLUE}💡 This demonstrates testing Unity game logic with mocking, independently of Unity Editor${NC}"
    echo -e "${BLUE}🎯 Tested components:${NC}"
    echo "  • RPGMath utility functions"
    echo "  • StateMachine state transitions"
    echo "  • Vector2Utils array operations"
    echo "  • Mock-based testing approach"
else
    echo -e "${RED}❌ Some tests failed${NC}"
    exit 1
fi

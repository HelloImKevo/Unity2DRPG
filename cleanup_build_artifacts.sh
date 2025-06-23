#!/bin/bash

# Safe Build Artifact Cleanup Utility
# This script safely removes .NET build artifacts from the StandaloneTests directory only

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${YELLOW}🧹 Safe Build Artifact Cleanup Utility${NC}"

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

echo -e "${GREEN}✅ Security validation passed${NC}"

# Function to safely remove directories if they exist and are in the right location
safe_remove_dir() {
    local dir_path="$1"
    local full_path="$EXPECTED_TEST_DIR/$dir_path"
    
    if [ -d "$full_path" ]; then
        echo -e "${BLUE}🗑️  Removing: $full_path${NC}"
        rm -rf "$full_path"
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ Successfully removed: $dir_path${NC}"
        else
            echo -e "${RED}❌ Failed to remove: $dir_path${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  Directory not found (already clean): $dir_path${NC}"
    fi
}

# Function to safely remove files matching a pattern
safe_remove_files() {
    local pattern="$1"
    local count=$(find "$EXPECTED_TEST_DIR" -maxdepth 1 -name "$pattern" -type f | wc -l)
    
    if [ $count -gt 0 ]; then
        echo -e "${BLUE}🗑️  Removing $count file(s) matching: $pattern${NC}"
        find "$EXPECTED_TEST_DIR" -maxdepth 1 -name "$pattern" -type f -delete
        echo -e "${GREEN}✅ Successfully removed files matching: $pattern${NC}"
    else
        echo -e "${YELLOW}⚠️  No files found matching: $pattern${NC}"
    fi
}

echo -e "${BLUE}📁 Cleaning build artifacts in: $EXPECTED_TEST_DIR${NC}"

# Clean common .NET build directories
safe_remove_dir "bin"
safe_remove_dir "obj"

# Clean common .NET build files
safe_remove_files "*.dll"
safe_remove_files "*.pdb"
safe_remove_files "*.deps.json"
safe_remove_files "*.runtimeconfig.json"
safe_remove_files "nunit_random_seed.tmp"

echo -e "${GREEN}✅ Cleanup completed successfully!${NC}"
echo -e "${BLUE}💡 Your repository should now be free of build artifacts${NC}"

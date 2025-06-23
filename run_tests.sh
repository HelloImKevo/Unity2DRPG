#!/bin/bash

# Unity Test Runner Script
# This script provides easy command-line access to Unity tests

# Configuration
PROJECT_PATH="$(pwd)/BraveryRPG"
RESULTS_DIR="$PROJECT_PATH/TestResults"
LOGS_DIR="$PROJECT_PATH/TestLogs"

# Function to automatically detect Unity installation
detect_unity_path() {
    local unity_path=""
    
    # Method 1: Check if UNITY_PATH environment variable is set
    if [ ! -z "$UNITY_PATH" ] && [ -f "$UNITY_PATH" ]; then
        echo "$UNITY_PATH"
        return 0
    fi
    
    # Method 2: Look for Unity Hub installations (most common)
    local hub_editors_dir="/Applications/Unity/Hub/Editor"
    if [ -d "$hub_editors_dir" ]; then
        # Find the latest version (sort by version number)
        local latest_version=$(ls "$hub_editors_dir" 2>/dev/null | grep -E '^[0-9]+\.[0-9]+\.[0-9]+[a-z]*[0-9]*$' | sort -V | tail -1)
        if [ ! -z "$latest_version" ]; then
            unity_path="$hub_editors_dir/$latest_version/Unity.app/Contents/MacOS/Unity"
            if [ -f "$unity_path" ]; then
                echo "$unity_path"
                return 0
            fi
        fi
    fi
    
    # Method 3: Check common standalone installation paths
    local standalone_paths=(
        "/Applications/Unity/Unity.app/Contents/MacOS/Unity"
        "/Applications/Unity.app/Contents/MacOS/Unity"
    )
    
    for path in "${standalone_paths[@]}"; do
        if [ -f "$path" ]; then
            echo "$path"
            return 0
        fi
    done
    
    # Method 4: Use 'which' command if Unity is in PATH
    local which_unity=$(which unity 2>/dev/null)
    if [ ! -z "$which_unity" ] && [ -f "$which_unity" ]; then
        echo "$which_unity"
        return 0
    fi
    
    # Method 5: Search using 'find' (slower but comprehensive)
    local found_unity=$(find /Applications -name "Unity" -type f -path "*/Contents/MacOS/Unity" 2>/dev/null | head -1)
    if [ ! -z "$found_unity" ] && [ -f "$found_unity" ]; then
        echo "$found_unity"
        return 0
    fi
    
    return 1
}

# Detect Unity path automatically
UNITY_PATH=$(detect_unity_path)

# Create output directories if they don't exist
mkdir -p "$RESULTS_DIR"
mkdir -p "$LOGS_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YIGHLIGHT='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YIGHLIGHT}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if Unity is available
check_unity() {
    if [ -z "$UNITY_PATH" ] || [ ! -f "$UNITY_PATH" ]; then
        print_error "Unity not found!"
        print_error ""
        print_error "Searched the following locations:"
        print_error "  1. Environment variable: \$UNITY_PATH"
        print_error "  2. Unity Hub: /Applications/Unity/Hub/Editor/*/Unity.app"
        print_error "  3. Standalone: /Applications/Unity/Unity.app"
        print_error "  4. System PATH: \$(which unity)"
        print_error "  5. Applications folder search"
        print_error ""
        print_error "Solutions:"
        print_error "  1. Set environment variable: export UNITY_PATH='/path/to/Unity'"
        print_error "  2. Install Unity via Unity Hub"
        print_error "  3. Add Unity to your PATH"
        print_error ""
        print_error "To set UNITY_PATH permanently, add this to your ~/.zshrc:"
        print_error "  export UNITY_PATH='/Applications/Unity/Hub/Editor/XXXX.X.XXfX/Unity.app/Contents/MacOS/Unity'"
        exit 1
    fi
    
    print_success "Found Unity at: $UNITY_PATH"
    
    # Display Unity version if possible
    local version_info=$("$UNITY_PATH" -version 2>/dev/null | head -1)
    if [ ! -z "$version_info" ]; then
        print_status "Unity version: $version_info"
    fi
}

# Function to run tests with fallback options
run_tests() {
    local test_filter="$1"
    local test_name="$2"
    local results_file="$RESULTS_DIR/${test_name}_Results.xml"
    local log_file="$LOGS_DIR/${test_name}_Log.txt"
    
    print_status "Running $test_name tests..."
    
    # Check for existing Unity instances that might interfere
    if check_unity_running; then
        print_warning "⚠️  Unity Editor is running with this project"
        print_status "Batch mode tests may conflict with the open editor."
        print_status ""
        print_status "Options:"
        print_status "  1. Close Unity Editor and run this command again"
        print_status "  2. Use interactive mode: ./run_tests.sh interactive"
        print_status "  3. Continue anyway (may cause conflicts)"
        echo ""
        read -p "Continue with batch mode anyway? (y/N): " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            print_status "Cancelled by user. Try: ./run_tests.sh interactive"
            return 1
        fi
        print_warning "Proceeding with batch mode despite running Unity instance..."
    elif [ $? -eq 2 ]; then
        print_warning "⚠️  Unity Editor is running with a different project"
        print_status "This should not interfere with batch mode, continuing..."
    fi
    
    print_status "Results will be saved to: $results_file"
    print_status "Logs will be saved to: $log_file"
    
    # First attempt: Standard batch mode
    print_status "Attempting batch mode test execution..."
    "$UNITY_PATH" \
        -batchmode \
        -quit \
        -projectPath "$PROJECT_PATH" \
        -runTests \
        -testPlatform EditMode \
        -testFilter "$test_filter" \
        -testResults "$results_file" \
        -logFile "$log_file"
    
    local exit_code=$?
    
    # If batch mode failed, try with no-graphics mode
    if [ $exit_code -ne 0 ]; then
        print_warning "Batch mode failed, trying with no-graphics mode..."
        local log_file_ng="$LOGS_DIR/${test_name}_NoGraphics_Log.txt"
        "$UNITY_PATH" \
            -batchmode \
            -nographics \
            -quit \
            -projectPath "$PROJECT_PATH" \
            -runTests \
            -testPlatform EditMode \
            -testFilter "$test_filter" \
            -testResults "$results_file" \
            -logFile "$log_file_ng"
        
        exit_code=$?
    fi
    
    # If both batch modes failed, suggest interactive mode
    if [ $exit_code -ne 0 ]; then
        print_error "Batch mode tests failed. This might be due to:"
        print_error "  1. Package manager certificate issues"
        print_error "  2. Missing dependencies"
        print_error "  3. Project compilation errors"
        print_error ""
        print_status "Suggestions:"
        print_status "  1. Open Unity Editor manually and run tests via Test Runner window"
        print_status "  2. Use interactive mode: ./run_tests.sh interactive"
        print_status "  3. Check the log files for detailed error information"
        print_error "Check the log file for details: $log_file"
        
        # Show last few lines of error log
        if [ -f "$log_file" ]; then
            print_status "Last 10 lines of error log:"
            tail -10 "$log_file"
        fi
    else
        print_success "$test_name tests completed successfully!"
        if [ -f "$results_file" ]; then
            # Extract test summary from results
            local passed=$(grep -o 'passed="[0-9]*"' "$results_file" | grep -o '[0-9]*' | head -1)
            local failed=$(grep -o 'failed="[0-9]*"' "$results_file" | grep -o '[0-9]*' | head -1)
            local total=$(grep -o 'total="[0-9]*"' "$results_file" | grep -o '[0-9]*' | head -1)
            
            if [ ! -z "$total" ]; then
                print_success "Test Summary: $passed passed, $failed failed, $total total"
            fi
        fi
    fi
    
    return $exit_code
}

# Function to check if Unity is running with our project
check_unity_running() {
    local project_name="BraveryRPG"
    
    # Check if Unity is running and get process info
    local unity_processes=$(pgrep -fl "Unity" 2>/dev/null | grep -v "Unity Hub")
    
    if [ -z "$unity_processes" ]; then
        return 1  # No Unity processes found
    fi
    
    # Try to find Unity process with our project using lsof (list open files)
    local project_process=$(lsof -c Unity 2>/dev/null | grep "$PROJECT_PATH" | head -1)
    
    if [ ! -z "$project_process" ]; then
        print_success "✅ Found Unity Editor already running with this project"
        return 0  # Unity is running with our project
    fi
    
    # Alternative check: look for Unity processes and check command line args
    local unity_with_project=$(pgrep -fl "Unity.*$project_name" 2>/dev/null)
    if [ ! -z "$unity_with_project" ]; then
        print_success "✅ Found Unity Editor running with project: $project_name"
        return 0
    fi
    
    return 2  # Unity is running but not with our project
}

# Function to run tests through existing Unity instance
run_tests_via_existing_unity() {
    local test_filter="$1"
    local output_name="$2"
    
    print_status "🔗 Attempting to run tests through existing Unity instance..."
    
    # Use AppleScript to bring Unity to front and show instructions
    osascript -e 'tell application "Unity" to activate' 2>/dev/null || true
    
    print_success "✅ Unity Editor is now in focus"
    print_status "📋 To run tests manually:"
    print_status "  1. Go to Window → General → Test Runner"
    print_status "  2. Switch to EditMode tab"
    if [ ! -z "$test_filter" ]; then
        print_status "  3. Filter tests by: $test_filter"
    fi
    print_status "  4. Click 'Run All' or select specific tests"
    print_status "  5. View results in the Test Runner window"
    
    echo ""
    print_status "💡 Alternative: Use Unity's menu 'Tools → Run All Tests' (if available)"
    print_status "💡 Or run: ./run_tests.sh entity-stats (for batch mode)"
}

# Function to run tests interactively (opens Unity Editor or uses existing)
run_tests_interactive() {
    local test_filter="$1"
    
    print_status "🔍 Checking for existing Unity instances..."
    
    if check_unity_running; then
        # Unity is already running with our project
        run_tests_via_existing_unity "$test_filter"
        return 0
    elif [ $? -eq 2 ]; then
        # Unity is running but with different project
        print_warning "⚠️  Unity Editor is running with a different project"
        print_status "Options:"
        print_status "  1. Close the current Unity project and run this command again"
        print_status "  2. Use batch mode: ./run_tests.sh entity-stats"
        print_status "  3. Continue anyway (will try to open new instance)"
        echo ""
        read -p "Continue with new instance? (y/N): " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            print_status "Cancelled by user"
            return 1
        fi
    fi
    
    # No Unity running with our project, start new instance
    print_status "🚀 Opening Unity Editor for interactive testing..."
    if [ ! -z "$test_filter" ]; then
        print_status "Focus on test filter: $test_filter"
    fi
    
    # Open Unity with the project, Unity Test Runner will be available
    "$UNITY_PATH" -projectPath "$PROJECT_PATH" &
    
    print_success "Unity Editor is starting..."
    print_status "To run tests:"
    print_status "  1. Go to Window > General > Test Runner"
    print_status "  2. Switch to EditMode tab"
    if [ ! -z "$test_filter" ]; then
        print_status "  3. Filter tests by: $test_filter"
    fi
    print_status "  4. Click 'Run All' or select specific tests"
}

# Function to display help
show_help() {
    echo "Unity Test Runner Script"
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  all              Run all Edit Mode tests (batch mode)"
    echo "  entity-stats     Run Entity_Stats tests only (batch mode)"
    echo "  interactive      Open Unity Editor for interactive testing (smart instance detection)"
    echo "  force-new        Force open new Unity instance (ignores existing instances)"
    echo "  status           Show Unity process status and recommendations"
    echo "  diagnose         Diagnose common Unity testing issues"
    echo "  help             Show this help message"
    echo "  clean            Clean test output files"
    echo "  results          Display latest test results"
    echo "  unity-info       Show detected Unity installation info"
    echo ""
    echo "Environment Variables:"
    echo "  UNITY_PATH       Override Unity executable path"
    echo "                   Example: export UNITY_PATH='/path/to/Unity'"
    echo ""
    echo "Examples:"
    echo "  $0 all"
    echo "  $0 entity-stats"
    echo "  $0 interactive      # Uses existing Unity if available"
    echo "  $0 force-new       # Always opens new Unity instance"
    echo "  $0 diagnose"
    echo "  $0 results"
    echo "  $0 unity-info"
    echo ""
    echo "  # Use specific Unity version:"
    echo "  UNITY_PATH='/Applications/Unity/Hub/Editor/6000.1.2f1/Unity.app/Contents/MacOS/Unity' $0 entity-stats"
}

# Function to clean test outputs
clean_outputs() {
    print_status "Cleaning test output files..."
    rm -rf "$RESULTS_DIR"/*
    rm -rf "$LOGS_DIR"/*
    print_success "Test output files cleaned"
}

# Function to show Unity installation info
show_unity_info() {
    print_status "Unity Installation Detection Results:"
    echo ""
    
    if [ ! -z "$UNITY_PATH" ] && [ -f "$UNITY_PATH" ]; then
        print_success "✅ Unity found at: $UNITY_PATH"
        
        # Try to get version info
        local version_info=$("$UNITY_PATH" -version 2>/dev/null | head -1)
        if [ ! -z "$version_info" ]; then
            print_status "📋 Version: $version_info"
        fi
        
        # Check if it's from Unity Hub
        if [[ "$UNITY_PATH" == *"/Unity/Hub/Editor/"* ]]; then
            local version_dir=$(echo "$UNITY_PATH" | sed 's|.*/Unity/Hub/Editor/\([^/]*\)/.*|\1|')
            print_status "🔧 Installation type: Unity Hub (version $version_dir)"
        else
            print_status "🔧 Installation type: Standalone"
        fi
        
        # Show file info
        print_status "📁 Executable size: $(du -h "$UNITY_PATH" 2>/dev/null | cut -f1)"
        print_status "📅 Last modified: $(date -r "$UNITY_PATH" 2>/dev/null)"
    else
        print_error "❌ Unity not found in any standard location"
        echo ""
        print_status "🔍 Searched locations:"
        echo "   • Environment variable: \$UNITY_PATH"
        echo "   • Unity Hub: /Applications/Unity/Hub/Editor/*/Unity.app"
        echo "   • Standalone: /Applications/Unity/Unity.app"
        echo "   • System PATH: \$(which unity)"
        echo "   • Applications folder search"
    fi
    
    echo ""
    print_status "🔧 Available Unity Hub installations:"
    local hub_dir="/Applications/Unity/Hub/Editor"
    if [ -d "$hub_dir" ]; then
        local versions=$(ls "$hub_dir" 2>/dev/null | grep -E '^[0-9]+\.[0-9]+\.[0-9]+[a-z]*[0-9]*$' | sort -V)
        if [ ! -z "$versions" ]; then
            echo "$versions" | while read version; do
                local unity_exe="$hub_dir/$version/Unity.app/Contents/MacOS/Unity"
                if [ -f "$unity_exe" ]; then
                    echo "   ✅ $version (at $unity_exe)"
                else
                    echo "   ❌ $version (incomplete installation)"
                fi
            done
        else
            echo "   No versions found"
        fi
    else
        echo "   Unity Hub not installed"
    fi
}

# Function to display results
show_results() {
    print_status "Latest test results:"
    if [ -d "$RESULTS_DIR" ] && [ "$(ls -A $RESULTS_DIR)" ]; then
        for file in "$RESULTS_DIR"/*.xml; do
            if [ -f "$file" ]; then
                echo ""
                print_status "Results from $(basename "$file"):"
                # Extract key information from XML
                grep -o 'passed="[0-9]*"' "$file" | head -1
                grep -o 'failed="[0-9]*"' "$file" | head -1
                grep -o 'total="[0-9]*"' "$file" | head -1
            fi
        done
    else
        print_warning "No test results found. Run tests first."
    fi
}

# Function to diagnose common Unity testing issues
diagnose_issues() {
    print_status "🔍 Diagnosing Unity testing issues..."
    echo ""
    
    # Check Unity project validity
    if [ ! -f "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt" ]; then
        print_error "❌ Not a valid Unity project (missing ProjectVersion.txt)"
        return 1
    fi
    
    local project_version=$(grep "m_EditorVersion:" "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt" | cut -d' ' -f2)
    print_status "📋 Project Unity version: $project_version"
    
    # Check Unity version compatibility
    local unity_version=$("$UNITY_PATH" -version 2>/dev/null | head -1)
    print_status "📋 Editor Unity version: $unity_version"
    
    # Check for common issues
    print_status "🔍 Checking for common issues:"
    
    # Check if Unity is running
    print_status "🔍 Checking Unity process status:"
    local unity_status=$(check_unity_running)
    local unity_check_result=$?
    
    case $unity_check_result in
        0)
            print_success "✅ Unity Editor is running with this project"
            print_status "   Interactive mode will use existing instance"
            ;;
        1)
            print_success "✅ No Unity Editor instances running"
            print_status "   Batch mode and interactive mode available"
            ;;
        2)
            print_warning "⚠️  Unity Editor is running with a different project"
            print_status "   Batch mode may conflict, interactive mode will prompt"
            ;;
    esac
    
    # Check Test Framework package
    if [ -f "$PROJECT_PATH/Packages/manifest.json" ]; then
        if grep -q "com.unity.test-framework" "$PROJECT_PATH/Packages/manifest.json"; then
            local test_version=$(grep "com.unity.test-framework" "$PROJECT_PATH/Packages/manifest.json" | cut -d'"' -f4)
            print_success "✅ Test Framework package found: $test_version"
        else
            print_error "❌ Test Framework package not found in manifest.json"
        fi
    fi
    
    # Check for test assembly
    if [ -f "$PROJECT_PATH/Assets/Scripts/Tests.asmdef" ]; then
        print_success "✅ Tests assembly definition found"
    else
        print_warning "⚠️  Tests.asmdef not found - tests might not be recognized"
    fi
    
    # Check for test files
    local test_files=$(find "$PROJECT_PATH/Assets" -name "*Tests.cs" 2>/dev/null | wc -l)
    print_status "📊 Found $test_files test files"
    
    # Check network connectivity (for package manager)
    print_status "🌐 Testing network connectivity to Unity services..."
    if ping -c 1 packages.unity.com > /dev/null 2>&1; then
        print_success "✅ Can reach Unity package servers"
    else
        print_warning "⚠️  Cannot reach Unity package servers (might cause package manager issues)"
    fi
    
    # Check for Unity Hub
    if [ -d "/Applications/Unity/Hub" ]; then
        print_success "✅ Unity Hub installation found"
    else
        print_warning "⚠️  Unity Hub not found (using standalone Unity installation)"
    fi
    
    echo ""
    print_status "🔧 Recommendations:"
    echo "  1. If certificate errors occur, try running Unity Editor manually first"
    echo "  2. For package manager issues, try: ./run_tests.sh interactive"
    echo "  3. Ensure firewall/antivirus isn't blocking Unity"
    echo "  4. Check Unity Console (Window > Console) for detailed errors"
}

# Function to show current Unity status and recommendations
show_status() {
    print_status "🔍 Checking Unity process status..."
    
    local unity_status=$(check_unity_running)
    local unity_check_result=$?
    
    case $unity_check_result in
        0)
            print_success "✅ Unity Editor is running with this project"
            print_status "📋 Recommendations:"
            print_status "  • Use: ./run_tests.sh interactive (will use existing instance)"
            print_status "  • Or run tests manually via Window → General → Test Runner"
            print_status "  • Avoid batch mode while editor is open"
            ;;
        1)
            print_success "✅ No Unity Editor instances running"
            print_status "📋 Recommendations:"
            print_status "  • Use: ./run_tests.sh all (for comprehensive testing)"
            print_status "  • Use: ./run_tests.sh entity-stats (for targeted testing)"
            print_status "  • Use: ./run_tests.sh interactive (to open Unity Editor)"
            ;;
        2)
            print_warning "⚠️  Unity Editor is running with a different project"
            print_status "📋 Recommendations:"
            print_status "  • Close current Unity project first"
            print_status "  • Or use: ./run_tests.sh force-new (may cause conflicts)"
            print_status "  • Batch mode should work fine"
            ;;
    esac
    
    echo ""
    print_status "🔧 Available commands:"
    echo "  ./run_tests.sh status          # Show this status"
    echo "  ./run_tests.sh entity-stats    # Run tests (batch mode)"
    echo "  ./run_tests.sh interactive     # Smart Unity Editor integration"
    echo "  ./run_tests.sh diagnose        # Troubleshoot issues"
    echo "  ./run_tests.sh results         # Show latest test results"
}

# Function to force open new Unity instance (ignores existing instances)
run_tests_force_new() {
    local test_filter="$1"
    
    print_status "🚀 Force opening new Unity Editor instance..."
    
    if check_unity_running; then
        print_warning "⚠️  This will open a new Unity instance alongside the existing one"
        print_status "💡 Note: Only one Unity instance can have a project open at a time"
    fi
    
    if [ ! -z "$test_filter" ]; then
        print_status "Focus on test filter: $test_filter"
    fi
    
    # Open Unity with the project, Unity Test Runner will be available
    "$UNITY_PATH" -projectPath "$PROJECT_PATH" &
    
    print_success "New Unity Editor instance is starting..."
    print_status "To run tests:"
    print_status "  1. Go to Window > General > Test Runner"
    print_status "  2. Switch to EditMode tab"
    if [ ! -z "$test_filter" ]; then
        print_status "  3. Filter tests by: $test_filter"
    fi
    print_status "  4. Click 'Run All' or select specific tests"
}

# Main script logic
main() {
    case "${1:-help}" in
        "all")
            check_unity
            run_tests "" "AllTests"
            ;;
        "entity-stats")
            check_unity
            run_tests "Entity_StatsTests" "EntityStats"
            ;;
        "interactive")
            check_unity
            run_tests_interactive "Entity_StatsTests"
            ;;
        "force-new")
            check_unity
            run_tests_force_new "Entity_StatsTests"
            ;;
        "status")
            check_unity
            show_status
            ;;
        "diagnose")
            check_unity
            diagnose_issues
            ;;
        "clean")
            clean_outputs
            ;;
        "results")
            show_results
            ;;
        "unity-info")
            show_unity_info
            ;;
        "help"|*)
            show_help
            ;;
    esac
}

# Run main function with all arguments
main "$@"

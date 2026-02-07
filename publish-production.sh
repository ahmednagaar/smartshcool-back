#!/bin/bash
echo "========================================"
echo "Building Nafes.API for Production..."
echo "========================================"

cd "$(dirname "$0")"

echo "Cleaning previous builds..."
rm -rf publish

echo "Building in Release mode..."
dotnet publish -c Release -o ./publish

echo "Copying web.config to publish folder..."
cp web.config publish/web.config

echo "========================================"
echo "Build Complete!"
echo "Files ready in: $(pwd)/publish"
echo "========================================"

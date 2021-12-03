#!/bin/bash

VERSION=$1
echo "Preparing release $VERSION"

# Update .csproj file
# This regex switches all occurences of >0.21.0< to >0.22.0<
# So <PackageVersion>0.21.0</PackageVersion> becomes <PackageVersion>0.22.0</PackageVersion>
sed -i -r "s/>[0-9]+\.[0-9]+\.[0-9]+</>$VERSION</g" src/stream-chat-net/stream-chat-net.csproj

# Create changelog, plus commit all changes
# commmit-all: Commit all staged changes (csproj, VERSION), not just files affected by standard-version
# Tagging will done by the GitHub release step, so skip it
git branch "release-$VERSION"
git config --global user.name 'GH Actions' 
git config --global user.email 'release@getstream.io'
git add .
npx --yes standard-version@9.3.2 --release-as "$VERSION" --skip.tag --commit-all
current_branch=$(git symbolic-ref --short HEAD)
git push -u origin "$current_branch"

echo "Done!"

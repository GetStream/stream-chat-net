#!/bin/bash

VERSION=$1
echo "Preparing release $VERSION"

# Splitting the version: 0.1.2 -> an array of [0, 1, 2]
IFS=. read -r -a splitted <<< "$VERSION"

# Update VERSION file
echo """
MAJOR=${splitted[0]}
MINOR=${splitted[1]}
BUG=${splitted[2]}
""" > VERSION

# Update .csproj file
# This regex switches all occurences of >0.21.0< to >0.22.0<
# So <PackageVersion>0.21.0</PackageVersion> becomes <PackageVersion>0.22.0</PackageVersion>
sed -i -r "s/>[0-9]+\.[0-9]+\.[0-9]+</>$VERSION</g" src/stream-chat-net/stream-chat-net.csproj

# Update version in Client.cs
# "0.21.0" becomes "0.22.0", including the double quotes
sed -i -r "s/\"[0-9]+\.[0-9]+\.[0-9]+\"/\"$VERSION\"/g" src/stream-chat-net/Client.cs

# Create changelog, plus commit
# commmit-all: Commit all staged changes, not just files affected by standard-version
# Tagging will done by the GitHub release step, so skip it
git config --global user.name 'GH Actions' 
git config --global user.email 'release@getstream.io'
npx --yes standard-version --release-as "$VERSION" --skip.tag --commit-all

echo "Done!"

#!/bin/bash

VERSION=$1
echo "Preparing release $VERSION"

# Update .csproj file
# This regex switches all occurences of >0.21.0< to >0.22.0<
# So <PackageVersion>0.21.0</PackageVersion> becomes <PackageVersion>0.22.0</PackageVersion>
sed -i -r "s/>[0-9]+\.[0-9]+\.[0-9]+</>$VERSION</g" src/stream-chat-net/stream-chat-net.csproj

# Create changelog
# --skip.commit: We manually commit the changes
# --skip-tag: tagging will done by the GitHub release step, so skip it here
# --tag-prefix: by default it tries to compare v0.1.0 to v0.2.0. Since we do not prefix our tags with 'v'
# we set it to an empty string
npx --yes standard-version@9.3.2 --release-as "$VERSION" --skip.tag --skip.commit --tag-prefix=

git config --global user.name 'github-actions' 
git config --global user.email 'release@getstream.io'
git checkout -q -b "release-$VERSION"
git commit -am "chore(release): $VERSION"
git push -q -u origin "release-$VERSION"

echo "Done!"

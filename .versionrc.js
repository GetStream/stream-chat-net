const csprojUpdater = {
    VERSION_REGEX: /<Version>(.+)<\/Version>/,

    readVersion: function (contents) {
        const version = this.VERSION_REGEX.exec(contents)[1];
        return version;
    },

    writeVersion: function (contents, version) {
        return contents.replace(this.VERSION_REGEX.exec(contents)[0], `<Version>${version}</Version>`);
    }
}

module.exports = {
    bumpFiles: [{ filename: './src/stream-chat-net.csproj', updater: csprojUpdater }],
}

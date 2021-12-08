const Configuration = {
    extends: ['@commitlint/config-conventional'],
    /*
     * Functions that return true if commitlint should ignore the given message.
     * In our case, if the commit contains a large link, sometimes it cannot fit into
     * the max line length, so let's ignore it.
     */
    ignores: [(commit) => commit.includes('https://')],
}
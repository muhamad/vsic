### ABOUT
This tool is used to reduce the packages cache of visual studio installer by detecting the extra packages that might not be needed and move them out to another directory.

The tool detect that by grouping the package name, language and architecture and then keep the highest package version and move out the rest.

### USAGE
```
vsic <vs-installer-source> <existing-dir-to-move-deprecated-packages>
```

`<vs-installer-source>`: is the visual studio installer layout directory.

`<existing-dir-to-move-deprecated-packages>`: is the directory to move extra packages into, it should be empty so no error occur during moving packages.

### LICENSE
This project is licensed under MIT

Developed by Muhammad Aladdin
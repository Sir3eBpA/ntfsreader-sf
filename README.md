# ntfsreader-sf
**Optimizations done in this fork so far:**
- stack alloc for string search in a hot path where it was allocating a bunch of StringBuilders

- array pool used for path building using node indices

- IEnumerable to speed up the file lookup on a single thread and reduce memory usage as the whole chunk of meta was pretty big (talking in gigabytes here)

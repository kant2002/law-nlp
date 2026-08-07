# law-nlp

Set of tools for analyzing text corpus of the law domain.


How to get list of documents for given filter. 

```
[...document.getElementsByTagName("a")].filter((node) => node.attributes["href"].value.startsWith("/rus/docs/") && !node.attributes["href"].value.startsWith("/rus/docs/rss")).map(node => node.attributes["href"].value.replace("/rus/docs/", "")).join("\n")
```
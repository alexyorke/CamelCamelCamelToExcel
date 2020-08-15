# CamelCamelCamelToExcel
Convert CamelCamelCamel listings to an Excel spreadsheet. Well, a TSV file which can be added to Excel.

![Screenshot](https://github.com/alexyorke/CamelCamelCamelToExcel/raw/master/image.png)
![Screenshot](https://github.com/alexyorke/CamelCamelCamelToExcel/raw/c5cff5e22408115bbd6e8277237639aa38d8b6f8/image.png)

## Usage

```csharp
const string url = "https://camelcamelcamel.com/product/B07G82D89J?context=search";
var productPageBuilder = new ProductPageBuilder(url);
var productPage = productPageBuilder.Build();

var graph = productPage.Graph.Create();

var tsv = graph.Aggregate("", (current, point) => current + $"{point.X}\t{point.Y}\n");

File.WriteAllText("CamelCamelCamelGraph.tsv", tsv);
```

Output looks like this:

```
259.397	52.520676
259.83368	52.520676
260.2704	52.520676
260.7071	52.520676
261.14377	52.49581
261.58047	52.49581
262.01718	49.98456
262.45386	49.98456
262.89056	49.98456
263.32724	49.98456
263.76395	49.98456
264.20065	52.520676
264.63733	52.520676
265.07404	52.520676
265.51074	52.520676
...
```

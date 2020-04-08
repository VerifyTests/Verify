# Diff Tool Order


## Default

include: defaultDiffToolOrder


## Custom order

Set an `Verify.DiffToolOrder` with the preferred order of diff tool resolution. The value can be comma (`,`), pipe (`|`), or space separated.

For example `VisualStudio,Meld` will result in VisualStudio then Meld then all other tools being the order.
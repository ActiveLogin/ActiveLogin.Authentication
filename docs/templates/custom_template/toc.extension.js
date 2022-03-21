exports.preTransform = function (model) {
    // replace recursively the root namespace by ""
    transformItem(model, 1);
    return model;
  
    function transformItem(item, level) {
      if (item.name) {
        item.name = item.name.replace("ActiveLogin.Authentication.", '');
      } else {
        item.name = null;
      }
  
      if (item.items && item.items.length > 0) {
        var length = item.items.length;
        for (var i = 0; i < length; i++) {
          transformItem(item.items[i], level + 1);
        };
      }
    }
  }
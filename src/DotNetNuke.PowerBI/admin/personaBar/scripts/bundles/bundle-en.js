/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "http://localhost:8080/dist/scripts/bundles";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 41);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.React;

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

/**
 * Copyright (c) 2013-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

if (true) {
  var ReactIs = __webpack_require__(10);

  // By explicitly using `prop-types` you are opting into new development behavior.
  // http://fb.me/prop-types-in-prod
  var throwOnDirectAccess = true;
  module.exports = __webpack_require__(20)(ReactIs.isElement, throwOnDirectAccess);
} else {}


/***/ }),
/* 2 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.CommonComponents;

/***/ }),
/* 3 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var utils = {
  init: function init(utilities) {
    if (!utilities) {
      throw new Error("Utilities is undefined.");
    }

    this.utilities = utilities;
  },
  utilities: null
};
/* harmony default export */ __webpack_exports__["a"] = (utils);

/***/ }),
/* 4 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";

// EXPORTS
__webpack_require__.d(__webpack_exports__, "a", function() { return /* reexport */ settings; });

// CONCATENATED MODULE: ./src/constants/actionTypes/settings.js
var settingsActionTypes = {
  SWITCH_TAB: "SWITCH_TAB",
  UPDATED_WORKSPACE: "UPDATED_WORKSPACE",
  RETRIEVED_WORKSPACES: "RETRIEVED_WORKSPACES",
  CANCELLED_WORKSPACE_CLIENT_MODIFIED: "CANCELLED_WORKSPACE_CLIENT_MODIFIED",
  WORKSPACE_CLIENT_MODIFIED: "WORKSPACE_CLIENT_MODIFIED",
  PERMISSIONS_CLIENT_MODIFIED: "PERMISSIONS_CLIENT_MODIFIED",
  RETRIEVED_POWERBI_OBJECT_LIST: "RETRIEVED_POWERBI_OBJECT_LIST",
  SELECTED_POWERBI_OBJECT: "SELECTED_POWERBI_OBJECT",
  PERMISSIONS_CHANGED: "PERMISSIONS_CHANGED",
  INHERIT_PERMISSIONS_CHANGED: "INHERIT_PERMISSIONS_CHANGED"
};
/* harmony default export */ var settings = (settingsActionTypes);
// CONCATENATED MODULE: ./src/constants/actionTypes/index.js



/***/ }),
/* 5 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReactRedux;

/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


/*
  MIT License http://www.opensource.org/licenses/mit-license.php
  Author Tobias Koppers @sokra
*/
// css base code, injected by the css-loader
module.exports = function (useSourceMap) {
  var list = []; // return the list of modules as css string

  list.toString = function toString() {
    return this.map(function (item) {
      var content = cssWithMappingToString(item, useSourceMap);

      if (item[2]) {
        return '@media ' + item[2] + '{' + content + '}';
      } else {
        return content;
      }
    }).join('');
  }; // import a list of modules into the list


  list.i = function (modules, mediaQuery) {
    if (typeof modules === 'string') {
      modules = [[null, modules, '']];
    }

    var alreadyImportedModules = {};

    for (var i = 0; i < this.length; i++) {
      var id = this[i][0];

      if (id != null) {
        alreadyImportedModules[id] = true;
      }
    }

    for (i = 0; i < modules.length; i++) {
      var item = modules[i]; // skip already imported module
      // this implementation is not 100% perfect for weird media query combinations
      // when a module is imported multiple times with different media queries.
      // I hope this will never occur (Hey this way we have smaller bundles)

      if (item[0] == null || !alreadyImportedModules[item[0]]) {
        if (mediaQuery && !item[2]) {
          item[2] = mediaQuery;
        } else if (mediaQuery) {
          item[2] = '(' + item[2] + ') and (' + mediaQuery + ')';
        }

        list.push(item);
      }
    }
  };

  return list;
};

function cssWithMappingToString(item, useSourceMap) {
  var content = item[1] || '';
  var cssMapping = item[3];

  if (!cssMapping) {
    return content;
  }

  if (useSourceMap && typeof btoa === 'function') {
    var sourceMapping = toComment(cssMapping);
    var sourceURLs = cssMapping.sources.map(function (source) {
      return '/*# sourceURL=' + cssMapping.sourceRoot + source + ' */';
    });
    return [content].concat(sourceURLs).concat([sourceMapping]).join('\n');
  }

  return [content].join('\n');
} // Adapted from convert-source-map (MIT)


function toComment(sourceMap) {
  // eslint-disable-next-line no-undef
  var base64 = btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap))));
  var data = 'sourceMappingURL=data:application/json;charset=utf-8;base64,' + base64;
  return '/*# ' + data + ' */';
}

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/

var stylesInDom = {};

var	memoize = function (fn) {
	var memo;

	return function () {
		if (typeof memo === "undefined") memo = fn.apply(this, arguments);
		return memo;
	};
};

var isOldIE = memoize(function () {
	// Test for IE <= 9 as proposed by Browserhacks
	// @see http://browserhacks.com/#hack-e71d8692f65334173fee715c222cb805
	// Tests for existence of standard globals is to allow style-loader
	// to operate correctly into non-standard environments
	// @see https://github.com/webpack-contrib/style-loader/issues/177
	return window && document && document.all && !window.atob;
});

var getTarget = function (target, parent) {
  if (parent){
    return parent.querySelector(target);
  }
  return document.querySelector(target);
};

var getElement = (function (fn) {
	var memo = {};

	return function(target, parent) {
                // If passing function in options, then use it for resolve "head" element.
                // Useful for Shadow Root style i.e
                // {
                //   insertInto: function () { return document.querySelector("#foo").shadowRoot }
                // }
                if (typeof target === 'function') {
                        return target();
                }
                if (typeof memo[target] === "undefined") {
			var styleTarget = getTarget.call(this, target, parent);
			// Special case to return head of iframe instead of iframe itself
			if (window.HTMLIFrameElement && styleTarget instanceof window.HTMLIFrameElement) {
				try {
					// This will throw an exception if access to iframe is blocked
					// due to cross-origin restrictions
					styleTarget = styleTarget.contentDocument.head;
				} catch(e) {
					styleTarget = null;
				}
			}
			memo[target] = styleTarget;
		}
		return memo[target]
	};
})();

var singleton = null;
var	singletonCounter = 0;
var	stylesInsertedAtTop = [];

var	fixUrls = __webpack_require__(25);

module.exports = function(list, options) {
	if (typeof DEBUG !== "undefined" && DEBUG) {
		if (typeof document !== "object") throw new Error("The style-loader cannot be used in a non-browser environment");
	}

	options = options || {};

	options.attrs = typeof options.attrs === "object" ? options.attrs : {};

	// Force single-tag solution on IE6-9, which has a hard limit on the # of <style>
	// tags it will allow on a page
	if (!options.singleton && typeof options.singleton !== "boolean") options.singleton = isOldIE();

	// By default, add <style> tags to the <head> element
        if (!options.insertInto) options.insertInto = "head";

	// By default, add <style> tags to the bottom of the target
	if (!options.insertAt) options.insertAt = "bottom";

	var styles = listToStyles(list, options);

	addStylesToDom(styles, options);

	return function update (newList) {
		var mayRemove = [];

		for (var i = 0; i < styles.length; i++) {
			var item = styles[i];
			var domStyle = stylesInDom[item.id];

			domStyle.refs--;
			mayRemove.push(domStyle);
		}

		if(newList) {
			var newStyles = listToStyles(newList, options);
			addStylesToDom(newStyles, options);
		}

		for (var i = 0; i < mayRemove.length; i++) {
			var domStyle = mayRemove[i];

			if(domStyle.refs === 0) {
				for (var j = 0; j < domStyle.parts.length; j++) domStyle.parts[j]();

				delete stylesInDom[domStyle.id];
			}
		}
	};
};

function addStylesToDom (styles, options) {
	for (var i = 0; i < styles.length; i++) {
		var item = styles[i];
		var domStyle = stylesInDom[item.id];

		if(domStyle) {
			domStyle.refs++;

			for(var j = 0; j < domStyle.parts.length; j++) {
				domStyle.parts[j](item.parts[j]);
			}

			for(; j < item.parts.length; j++) {
				domStyle.parts.push(addStyle(item.parts[j], options));
			}
		} else {
			var parts = [];

			for(var j = 0; j < item.parts.length; j++) {
				parts.push(addStyle(item.parts[j], options));
			}

			stylesInDom[item.id] = {id: item.id, refs: 1, parts: parts};
		}
	}
}

function listToStyles (list, options) {
	var styles = [];
	var newStyles = {};

	for (var i = 0; i < list.length; i++) {
		var item = list[i];
		var id = options.base ? item[0] + options.base : item[0];
		var css = item[1];
		var media = item[2];
		var sourceMap = item[3];
		var part = {css: css, media: media, sourceMap: sourceMap};

		if(!newStyles[id]) styles.push(newStyles[id] = {id: id, parts: [part]});
		else newStyles[id].parts.push(part);
	}

	return styles;
}

function insertStyleElement (options, style) {
	var target = getElement(options.insertInto)

	if (!target) {
		throw new Error("Couldn't find a style target. This probably means that the value for the 'insertInto' parameter is invalid.");
	}

	var lastStyleElementInsertedAtTop = stylesInsertedAtTop[stylesInsertedAtTop.length - 1];

	if (options.insertAt === "top") {
		if (!lastStyleElementInsertedAtTop) {
			target.insertBefore(style, target.firstChild);
		} else if (lastStyleElementInsertedAtTop.nextSibling) {
			target.insertBefore(style, lastStyleElementInsertedAtTop.nextSibling);
		} else {
			target.appendChild(style);
		}
		stylesInsertedAtTop.push(style);
	} else if (options.insertAt === "bottom") {
		target.appendChild(style);
	} else if (typeof options.insertAt === "object" && options.insertAt.before) {
		var nextSibling = getElement(options.insertAt.before, target);
		target.insertBefore(style, nextSibling);
	} else {
		throw new Error("[Style Loader]\n\n Invalid value for parameter 'insertAt' ('options.insertAt') found.\n Must be 'top', 'bottom', or Object.\n (https://github.com/webpack-contrib/style-loader#insertat)\n");
	}
}

function removeStyleElement (style) {
	if (style.parentNode === null) return false;
	style.parentNode.removeChild(style);

	var idx = stylesInsertedAtTop.indexOf(style);
	if(idx >= 0) {
		stylesInsertedAtTop.splice(idx, 1);
	}
}

function createStyleElement (options) {
	var style = document.createElement("style");

	if(options.attrs.type === undefined) {
		options.attrs.type = "text/css";
	}

	if(options.attrs.nonce === undefined) {
		var nonce = getNonce();
		if (nonce) {
			options.attrs.nonce = nonce;
		}
	}

	addAttrs(style, options.attrs);
	insertStyleElement(options, style);

	return style;
}

function createLinkElement (options) {
	var link = document.createElement("link");

	if(options.attrs.type === undefined) {
		options.attrs.type = "text/css";
	}
	options.attrs.rel = "stylesheet";

	addAttrs(link, options.attrs);
	insertStyleElement(options, link);

	return link;
}

function addAttrs (el, attrs) {
	Object.keys(attrs).forEach(function (key) {
		el.setAttribute(key, attrs[key]);
	});
}

function getNonce() {
	if (false) {}

	return __webpack_require__.nc;
}

function addStyle (obj, options) {
	var style, update, remove, result;

	// If a transform function was defined, run it on the css
	if (options.transform && obj.css) {
	    result = typeof options.transform === 'function'
		 ? options.transform(obj.css) 
		 : options.transform.default(obj.css);

	    if (result) {
	    	// If transform returns a value, use that instead of the original css.
	    	// This allows running runtime transformations on the css.
	    	obj.css = result;
	    } else {
	    	// If the transform function returns a falsy value, don't add this css.
	    	// This allows conditional loading of css
	    	return function() {
	    		// noop
	    	};
	    }
	}

	if (options.singleton) {
		var styleIndex = singletonCounter++;

		style = singleton || (singleton = createStyleElement(options));

		update = applyToSingletonTag.bind(null, style, styleIndex, false);
		remove = applyToSingletonTag.bind(null, style, styleIndex, true);

	} else if (
		obj.sourceMap &&
		typeof URL === "function" &&
		typeof URL.createObjectURL === "function" &&
		typeof URL.revokeObjectURL === "function" &&
		typeof Blob === "function" &&
		typeof btoa === "function"
	) {
		style = createLinkElement(options);
		update = updateLink.bind(null, style, options);
		remove = function () {
			removeStyleElement(style);

			if(style.href) URL.revokeObjectURL(style.href);
		};
	} else {
		style = createStyleElement(options);
		update = applyToTag.bind(null, style);
		remove = function () {
			removeStyleElement(style);
		};
	}

	update(obj);

	return function updateStyle (newObj) {
		if (newObj) {
			if (
				newObj.css === obj.css &&
				newObj.media === obj.media &&
				newObj.sourceMap === obj.sourceMap
			) {
				return;
			}

			update(obj = newObj);
		} else {
			remove();
		}
	};
}

var replaceText = (function () {
	var textStore = [];

	return function (index, replacement) {
		textStore[index] = replacement;

		return textStore.filter(Boolean).join('\n');
	};
})();

function applyToSingletonTag (style, index, remove, obj) {
	var css = remove ? "" : obj.css;

	if (style.styleSheet) {
		style.styleSheet.cssText = replaceText(index, css);
	} else {
		var cssNode = document.createTextNode(css);
		var childNodes = style.childNodes;

		if (childNodes[index]) style.removeChild(childNodes[index]);

		if (childNodes.length) {
			style.insertBefore(cssNode, childNodes[index]);
		} else {
			style.appendChild(cssNode);
		}
	}
}

function applyToTag (style, obj) {
	var css = obj.css;
	var media = obj.media;

	if(media) {
		style.setAttribute("media", media)
	}

	if(style.styleSheet) {
		style.styleSheet.cssText = css;
	} else {
		while(style.firstChild) {
			style.removeChild(style.firstChild);
		}

		style.appendChild(document.createTextNode(css));
	}
}

function updateLink (link, options, obj) {
	var css = obj.css;
	var sourceMap = obj.sourceMap;

	/*
		If convertToAbsoluteUrls isn't defined, but sourcemaps are enabled
		and there is no publicPath defined then lets turn convertToAbsoluteUrls
		on by default.  Otherwise default to the convertToAbsoluteUrls option
		directly
	*/
	var autoFixUrls = options.convertToAbsoluteUrls === undefined && sourceMap;

	if (options.convertToAbsoluteUrls || autoFixUrls) {
		css = fixUrls(css);
	}

	if (sourceMap) {
		// http://stackoverflow.com/a/26603875
		css += "\n/*# sourceMappingURL=data:application/json;base64," + btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))) + " */";
	}

	var blob = new Blob([css], { type: "text/css" });

	var oldSrc = link.href;

	link.href = URL.createObjectURL(blob);

	if(oldSrc) URL.revokeObjectURL(oldSrc);
}


/***/ }),
/* 8 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.Redux;

/***/ }),
/* 9 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(0);
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var redux_devtools__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(15);
/* harmony import */ var redux_devtools__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(redux_devtools__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var redux_devtools_log_monitor__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(16);
/* harmony import */ var redux_devtools_log_monitor__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(redux_devtools_log_monitor__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var redux_devtools_dock_monitor__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(17);
/* harmony import */ var redux_devtools_dock_monitor__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(redux_devtools_dock_monitor__WEBPACK_IMPORTED_MODULE_3__);




/* harmony default export */ __webpack_exports__["a"] = (Object(redux_devtools__WEBPACK_IMPORTED_MODULE_1__["createDevTools"])(react__WEBPACK_IMPORTED_MODULE_0___default.a.createElement(redux_devtools_dock_monitor__WEBPACK_IMPORTED_MODULE_3___default.a, {
  toggleVisibilityKey: "ctrl-h",
  changePositionKey: "ctrl-q",
  defaultIsVisible: false
}, react__WEBPACK_IMPORTED_MODULE_0___default.a.createElement(redux_devtools_log_monitor__WEBPACK_IMPORTED_MODULE_2___default.a, null))));

/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


if (false) {} else {
  module.exports = __webpack_require__(19);
}


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/**
 * Copyright (c) 2013-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */



var ReactPropTypesSecret = 'SECRET_DO_NOT_PASS_THIS_OR_YOU_WILL_BE_FIRED';

module.exports = ReactPropTypesSecret;


/***/ }),
/* 12 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReactDOM;

/***/ }),
/* 13 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReduxThunk;

/***/ }),
/* 14 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReduxImmutableStateInvariant;

/***/ }),
/* 15 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReduxDevTools;

/***/ }),
/* 16 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReduxDevToolsLogMonitor;

/***/ }),
/* 17 */
/***/ (function(module, exports) {

module.exports = window.dnn.nodeModules.ReduxDevToolsDockMonitor;

/***/ }),
/* 18 */
/***/ (function(module, exports, __webpack_require__) {

/* eslint-disable no-undef */
if (false) {} else {
  module.exports = __webpack_require__(40);
}

/***/ }),
/* 19 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/** @license React v16.8.6
 * react-is.development.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */





if (true) {
  (function() {
'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

// The Symbol used to tag the ReactElement-like types. If there is no native Symbol
// nor polyfill, then a plain number is used for performance.
var hasSymbol = typeof Symbol === 'function' && Symbol.for;

var REACT_ELEMENT_TYPE = hasSymbol ? Symbol.for('react.element') : 0xeac7;
var REACT_PORTAL_TYPE = hasSymbol ? Symbol.for('react.portal') : 0xeaca;
var REACT_FRAGMENT_TYPE = hasSymbol ? Symbol.for('react.fragment') : 0xeacb;
var REACT_STRICT_MODE_TYPE = hasSymbol ? Symbol.for('react.strict_mode') : 0xeacc;
var REACT_PROFILER_TYPE = hasSymbol ? Symbol.for('react.profiler') : 0xead2;
var REACT_PROVIDER_TYPE = hasSymbol ? Symbol.for('react.provider') : 0xeacd;
var REACT_CONTEXT_TYPE = hasSymbol ? Symbol.for('react.context') : 0xeace;
var REACT_ASYNC_MODE_TYPE = hasSymbol ? Symbol.for('react.async_mode') : 0xeacf;
var REACT_CONCURRENT_MODE_TYPE = hasSymbol ? Symbol.for('react.concurrent_mode') : 0xeacf;
var REACT_FORWARD_REF_TYPE = hasSymbol ? Symbol.for('react.forward_ref') : 0xead0;
var REACT_SUSPENSE_TYPE = hasSymbol ? Symbol.for('react.suspense') : 0xead1;
var REACT_MEMO_TYPE = hasSymbol ? Symbol.for('react.memo') : 0xead3;
var REACT_LAZY_TYPE = hasSymbol ? Symbol.for('react.lazy') : 0xead4;

function isValidElementType(type) {
  return typeof type === 'string' || typeof type === 'function' ||
  // Note: its typeof might be other than 'symbol' or 'number' if it's a polyfill.
  type === REACT_FRAGMENT_TYPE || type === REACT_CONCURRENT_MODE_TYPE || type === REACT_PROFILER_TYPE || type === REACT_STRICT_MODE_TYPE || type === REACT_SUSPENSE_TYPE || typeof type === 'object' && type !== null && (type.$$typeof === REACT_LAZY_TYPE || type.$$typeof === REACT_MEMO_TYPE || type.$$typeof === REACT_PROVIDER_TYPE || type.$$typeof === REACT_CONTEXT_TYPE || type.$$typeof === REACT_FORWARD_REF_TYPE);
}

/**
 * Forked from fbjs/warning:
 * https://github.com/facebook/fbjs/blob/e66ba20ad5be433eb54423f2b097d829324d9de6/packages/fbjs/src/__forks__/warning.js
 *
 * Only change is we use console.warn instead of console.error,
 * and do nothing when 'console' is not supported.
 * This really simplifies the code.
 * ---
 * Similar to invariant but only logs a warning if the condition is not met.
 * This can be used to log issues in development environments in critical
 * paths. Removing the logging code for production environments will keep the
 * same logic and follow the same code paths.
 */

var lowPriorityWarning = function () {};

{
  var printWarning = function (format) {
    for (var _len = arguments.length, args = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
      args[_key - 1] = arguments[_key];
    }

    var argIndex = 0;
    var message = 'Warning: ' + format.replace(/%s/g, function () {
      return args[argIndex++];
    });
    if (typeof console !== 'undefined') {
      console.warn(message);
    }
    try {
      // --- Welcome to debugging React ---
      // This error was thrown as a convenience so that you can use this stack
      // to find the callsite that caused this warning to fire.
      throw new Error(message);
    } catch (x) {}
  };

  lowPriorityWarning = function (condition, format) {
    if (format === undefined) {
      throw new Error('`lowPriorityWarning(condition, format, ...args)` requires a warning ' + 'message argument');
    }
    if (!condition) {
      for (var _len2 = arguments.length, args = Array(_len2 > 2 ? _len2 - 2 : 0), _key2 = 2; _key2 < _len2; _key2++) {
        args[_key2 - 2] = arguments[_key2];
      }

      printWarning.apply(undefined, [format].concat(args));
    }
  };
}

var lowPriorityWarning$1 = lowPriorityWarning;

function typeOf(object) {
  if (typeof object === 'object' && object !== null) {
    var $$typeof = object.$$typeof;
    switch ($$typeof) {
      case REACT_ELEMENT_TYPE:
        var type = object.type;

        switch (type) {
          case REACT_ASYNC_MODE_TYPE:
          case REACT_CONCURRENT_MODE_TYPE:
          case REACT_FRAGMENT_TYPE:
          case REACT_PROFILER_TYPE:
          case REACT_STRICT_MODE_TYPE:
          case REACT_SUSPENSE_TYPE:
            return type;
          default:
            var $$typeofType = type && type.$$typeof;

            switch ($$typeofType) {
              case REACT_CONTEXT_TYPE:
              case REACT_FORWARD_REF_TYPE:
              case REACT_PROVIDER_TYPE:
                return $$typeofType;
              default:
                return $$typeof;
            }
        }
      case REACT_LAZY_TYPE:
      case REACT_MEMO_TYPE:
      case REACT_PORTAL_TYPE:
        return $$typeof;
    }
  }

  return undefined;
}

// AsyncMode is deprecated along with isAsyncMode
var AsyncMode = REACT_ASYNC_MODE_TYPE;
var ConcurrentMode = REACT_CONCURRENT_MODE_TYPE;
var ContextConsumer = REACT_CONTEXT_TYPE;
var ContextProvider = REACT_PROVIDER_TYPE;
var Element = REACT_ELEMENT_TYPE;
var ForwardRef = REACT_FORWARD_REF_TYPE;
var Fragment = REACT_FRAGMENT_TYPE;
var Lazy = REACT_LAZY_TYPE;
var Memo = REACT_MEMO_TYPE;
var Portal = REACT_PORTAL_TYPE;
var Profiler = REACT_PROFILER_TYPE;
var StrictMode = REACT_STRICT_MODE_TYPE;
var Suspense = REACT_SUSPENSE_TYPE;

var hasWarnedAboutDeprecatedIsAsyncMode = false;

// AsyncMode should be deprecated
function isAsyncMode(object) {
  {
    if (!hasWarnedAboutDeprecatedIsAsyncMode) {
      hasWarnedAboutDeprecatedIsAsyncMode = true;
      lowPriorityWarning$1(false, 'The ReactIs.isAsyncMode() alias has been deprecated, ' + 'and will be removed in React 17+. Update your code to use ' + 'ReactIs.isConcurrentMode() instead. It has the exact same API.');
    }
  }
  return isConcurrentMode(object) || typeOf(object) === REACT_ASYNC_MODE_TYPE;
}
function isConcurrentMode(object) {
  return typeOf(object) === REACT_CONCURRENT_MODE_TYPE;
}
function isContextConsumer(object) {
  return typeOf(object) === REACT_CONTEXT_TYPE;
}
function isContextProvider(object) {
  return typeOf(object) === REACT_PROVIDER_TYPE;
}
function isElement(object) {
  return typeof object === 'object' && object !== null && object.$$typeof === REACT_ELEMENT_TYPE;
}
function isForwardRef(object) {
  return typeOf(object) === REACT_FORWARD_REF_TYPE;
}
function isFragment(object) {
  return typeOf(object) === REACT_FRAGMENT_TYPE;
}
function isLazy(object) {
  return typeOf(object) === REACT_LAZY_TYPE;
}
function isMemo(object) {
  return typeOf(object) === REACT_MEMO_TYPE;
}
function isPortal(object) {
  return typeOf(object) === REACT_PORTAL_TYPE;
}
function isProfiler(object) {
  return typeOf(object) === REACT_PROFILER_TYPE;
}
function isStrictMode(object) {
  return typeOf(object) === REACT_STRICT_MODE_TYPE;
}
function isSuspense(object) {
  return typeOf(object) === REACT_SUSPENSE_TYPE;
}

exports.typeOf = typeOf;
exports.AsyncMode = AsyncMode;
exports.ConcurrentMode = ConcurrentMode;
exports.ContextConsumer = ContextConsumer;
exports.ContextProvider = ContextProvider;
exports.Element = Element;
exports.ForwardRef = ForwardRef;
exports.Fragment = Fragment;
exports.Lazy = Lazy;
exports.Memo = Memo;
exports.Portal = Portal;
exports.Profiler = Profiler;
exports.StrictMode = StrictMode;
exports.Suspense = Suspense;
exports.isValidElementType = isValidElementType;
exports.isAsyncMode = isAsyncMode;
exports.isConcurrentMode = isConcurrentMode;
exports.isContextConsumer = isContextConsumer;
exports.isContextProvider = isContextProvider;
exports.isElement = isElement;
exports.isForwardRef = isForwardRef;
exports.isFragment = isFragment;
exports.isLazy = isLazy;
exports.isMemo = isMemo;
exports.isPortal = isPortal;
exports.isProfiler = isProfiler;
exports.isStrictMode = isStrictMode;
exports.isSuspense = isSuspense;
  })();
}


/***/ }),
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/**
 * Copyright (c) 2013-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */



var ReactIs = __webpack_require__(10);
var assign = __webpack_require__(21);

var ReactPropTypesSecret = __webpack_require__(11);
var checkPropTypes = __webpack_require__(22);

var has = Function.call.bind(Object.prototype.hasOwnProperty);
var printWarning = function() {};

if (true) {
  printWarning = function(text) {
    var message = 'Warning: ' + text;
    if (typeof console !== 'undefined') {
      console.error(message);
    }
    try {
      // --- Welcome to debugging React ---
      // This error was thrown as a convenience so that you can use this stack
      // to find the callsite that caused this warning to fire.
      throw new Error(message);
    } catch (x) {}
  };
}

function emptyFunctionThatReturnsNull() {
  return null;
}

module.exports = function(isValidElement, throwOnDirectAccess) {
  /* global Symbol */
  var ITERATOR_SYMBOL = typeof Symbol === 'function' && Symbol.iterator;
  var FAUX_ITERATOR_SYMBOL = '@@iterator'; // Before Symbol spec.

  /**
   * Returns the iterator method function contained on the iterable object.
   *
   * Be sure to invoke the function with the iterable as context:
   *
   *     var iteratorFn = getIteratorFn(myIterable);
   *     if (iteratorFn) {
   *       var iterator = iteratorFn.call(myIterable);
   *       ...
   *     }
   *
   * @param {?object} maybeIterable
   * @return {?function}
   */
  function getIteratorFn(maybeIterable) {
    var iteratorFn = maybeIterable && (ITERATOR_SYMBOL && maybeIterable[ITERATOR_SYMBOL] || maybeIterable[FAUX_ITERATOR_SYMBOL]);
    if (typeof iteratorFn === 'function') {
      return iteratorFn;
    }
  }

  /**
   * Collection of methods that allow declaration and validation of props that are
   * supplied to React components. Example usage:
   *
   *   var Props = require('ReactPropTypes');
   *   var MyArticle = React.createClass({
   *     propTypes: {
   *       // An optional string prop named "description".
   *       description: Props.string,
   *
   *       // A required enum prop named "category".
   *       category: Props.oneOf(['News','Photos']).isRequired,
   *
   *       // A prop named "dialog" that requires an instance of Dialog.
   *       dialog: Props.instanceOf(Dialog).isRequired
   *     },
   *     render: function() { ... }
   *   });
   *
   * A more formal specification of how these methods are used:
   *
   *   type := array|bool|func|object|number|string|oneOf([...])|instanceOf(...)
   *   decl := ReactPropTypes.{type}(.isRequired)?
   *
   * Each and every declaration produces a function with the same signature. This
   * allows the creation of custom validation functions. For example:
   *
   *  var MyLink = React.createClass({
   *    propTypes: {
   *      // An optional string or URI prop named "href".
   *      href: function(props, propName, componentName) {
   *        var propValue = props[propName];
   *        if (propValue != null && typeof propValue !== 'string' &&
   *            !(propValue instanceof URI)) {
   *          return new Error(
   *            'Expected a string or an URI for ' + propName + ' in ' +
   *            componentName
   *          );
   *        }
   *      }
   *    },
   *    render: function() {...}
   *  });
   *
   * @internal
   */

  var ANONYMOUS = '<<anonymous>>';

  // Important!
  // Keep this list in sync with production version in `./factoryWithThrowingShims.js`.
  var ReactPropTypes = {
    array: createPrimitiveTypeChecker('array'),
    bool: createPrimitiveTypeChecker('boolean'),
    func: createPrimitiveTypeChecker('function'),
    number: createPrimitiveTypeChecker('number'),
    object: createPrimitiveTypeChecker('object'),
    string: createPrimitiveTypeChecker('string'),
    symbol: createPrimitiveTypeChecker('symbol'),

    any: createAnyTypeChecker(),
    arrayOf: createArrayOfTypeChecker,
    element: createElementTypeChecker(),
    elementType: createElementTypeTypeChecker(),
    instanceOf: createInstanceTypeChecker,
    node: createNodeChecker(),
    objectOf: createObjectOfTypeChecker,
    oneOf: createEnumTypeChecker,
    oneOfType: createUnionTypeChecker,
    shape: createShapeTypeChecker,
    exact: createStrictShapeTypeChecker,
  };

  /**
   * inlined Object.is polyfill to avoid requiring consumers ship their own
   * https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/is
   */
  /*eslint-disable no-self-compare*/
  function is(x, y) {
    // SameValue algorithm
    if (x === y) {
      // Steps 1-5, 7-10
      // Steps 6.b-6.e: +0 != -0
      return x !== 0 || 1 / x === 1 / y;
    } else {
      // Step 6.a: NaN == NaN
      return x !== x && y !== y;
    }
  }
  /*eslint-enable no-self-compare*/

  /**
   * We use an Error-like object for backward compatibility as people may call
   * PropTypes directly and inspect their output. However, we don't use real
   * Errors anymore. We don't inspect their stack anyway, and creating them
   * is prohibitively expensive if they are created too often, such as what
   * happens in oneOfType() for any type before the one that matched.
   */
  function PropTypeError(message) {
    this.message = message;
    this.stack = '';
  }
  // Make `instanceof Error` still work for returned errors.
  PropTypeError.prototype = Error.prototype;

  function createChainableTypeChecker(validate) {
    if (true) {
      var manualPropTypeCallCache = {};
      var manualPropTypeWarningCount = 0;
    }
    function checkType(isRequired, props, propName, componentName, location, propFullName, secret) {
      componentName = componentName || ANONYMOUS;
      propFullName = propFullName || propName;

      if (secret !== ReactPropTypesSecret) {
        if (throwOnDirectAccess) {
          // New behavior only for users of `prop-types` package
          var err = new Error(
            'Calling PropTypes validators directly is not supported by the `prop-types` package. ' +
            'Use `PropTypes.checkPropTypes()` to call them. ' +
            'Read more at http://fb.me/use-check-prop-types'
          );
          err.name = 'Invariant Violation';
          throw err;
        } else if ( true && typeof console !== 'undefined') {
          // Old behavior for people using React.PropTypes
          var cacheKey = componentName + ':' + propName;
          if (
            !manualPropTypeCallCache[cacheKey] &&
            // Avoid spamming the console because they are often not actionable except for lib authors
            manualPropTypeWarningCount < 3
          ) {
            printWarning(
              'You are manually calling a React.PropTypes validation ' +
              'function for the `' + propFullName + '` prop on `' + componentName  + '`. This is deprecated ' +
              'and will throw in the standalone `prop-types` package. ' +
              'You may be seeing this warning due to a third-party PropTypes ' +
              'library. See https://fb.me/react-warning-dont-call-proptypes ' + 'for details.'
            );
            manualPropTypeCallCache[cacheKey] = true;
            manualPropTypeWarningCount++;
          }
        }
      }
      if (props[propName] == null) {
        if (isRequired) {
          if (props[propName] === null) {
            return new PropTypeError('The ' + location + ' `' + propFullName + '` is marked as required ' + ('in `' + componentName + '`, but its value is `null`.'));
          }
          return new PropTypeError('The ' + location + ' `' + propFullName + '` is marked as required in ' + ('`' + componentName + '`, but its value is `undefined`.'));
        }
        return null;
      } else {
        return validate(props, propName, componentName, location, propFullName);
      }
    }

    var chainedCheckType = checkType.bind(null, false);
    chainedCheckType.isRequired = checkType.bind(null, true);

    return chainedCheckType;
  }

  function createPrimitiveTypeChecker(expectedType) {
    function validate(props, propName, componentName, location, propFullName, secret) {
      var propValue = props[propName];
      var propType = getPropType(propValue);
      if (propType !== expectedType) {
        // `propValue` being instance of, say, date/regexp, pass the 'object'
        // check, but we can offer a more precise error message here rather than
        // 'of type `object`'.
        var preciseType = getPreciseType(propValue);

        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + preciseType + '` supplied to `' + componentName + '`, expected ') + ('`' + expectedType + '`.'));
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createAnyTypeChecker() {
    return createChainableTypeChecker(emptyFunctionThatReturnsNull);
  }

  function createArrayOfTypeChecker(typeChecker) {
    function validate(props, propName, componentName, location, propFullName) {
      if (typeof typeChecker !== 'function') {
        return new PropTypeError('Property `' + propFullName + '` of component `' + componentName + '` has invalid PropType notation inside arrayOf.');
      }
      var propValue = props[propName];
      if (!Array.isArray(propValue)) {
        var propType = getPropType(propValue);
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + propType + '` supplied to `' + componentName + '`, expected an array.'));
      }
      for (var i = 0; i < propValue.length; i++) {
        var error = typeChecker(propValue, i, componentName, location, propFullName + '[' + i + ']', ReactPropTypesSecret);
        if (error instanceof Error) {
          return error;
        }
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createElementTypeChecker() {
    function validate(props, propName, componentName, location, propFullName) {
      var propValue = props[propName];
      if (!isValidElement(propValue)) {
        var propType = getPropType(propValue);
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + propType + '` supplied to `' + componentName + '`, expected a single ReactElement.'));
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createElementTypeTypeChecker() {
    function validate(props, propName, componentName, location, propFullName) {
      var propValue = props[propName];
      if (!ReactIs.isValidElementType(propValue)) {
        var propType = getPropType(propValue);
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + propType + '` supplied to `' + componentName + '`, expected a single ReactElement type.'));
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createInstanceTypeChecker(expectedClass) {
    function validate(props, propName, componentName, location, propFullName) {
      if (!(props[propName] instanceof expectedClass)) {
        var expectedClassName = expectedClass.name || ANONYMOUS;
        var actualClassName = getClassName(props[propName]);
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + actualClassName + '` supplied to `' + componentName + '`, expected ') + ('instance of `' + expectedClassName + '`.'));
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createEnumTypeChecker(expectedValues) {
    if (!Array.isArray(expectedValues)) {
      if (true) {
        if (arguments.length > 1) {
          printWarning(
            'Invalid arguments supplied to oneOf, expected an array, got ' + arguments.length + ' arguments. ' +
            'A common mistake is to write oneOf(x, y, z) instead of oneOf([x, y, z]).'
          );
        } else {
          printWarning('Invalid argument supplied to oneOf, expected an array.');
        }
      }
      return emptyFunctionThatReturnsNull;
    }

    function validate(props, propName, componentName, location, propFullName) {
      var propValue = props[propName];
      for (var i = 0; i < expectedValues.length; i++) {
        if (is(propValue, expectedValues[i])) {
          return null;
        }
      }

      var valuesString = JSON.stringify(expectedValues, function replacer(key, value) {
        var type = getPreciseType(value);
        if (type === 'symbol') {
          return String(value);
        }
        return value;
      });
      return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of value `' + String(propValue) + '` ' + ('supplied to `' + componentName + '`, expected one of ' + valuesString + '.'));
    }
    return createChainableTypeChecker(validate);
  }

  function createObjectOfTypeChecker(typeChecker) {
    function validate(props, propName, componentName, location, propFullName) {
      if (typeof typeChecker !== 'function') {
        return new PropTypeError('Property `' + propFullName + '` of component `' + componentName + '` has invalid PropType notation inside objectOf.');
      }
      var propValue = props[propName];
      var propType = getPropType(propValue);
      if (propType !== 'object') {
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type ' + ('`' + propType + '` supplied to `' + componentName + '`, expected an object.'));
      }
      for (var key in propValue) {
        if (has(propValue, key)) {
          var error = typeChecker(propValue, key, componentName, location, propFullName + '.' + key, ReactPropTypesSecret);
          if (error instanceof Error) {
            return error;
          }
        }
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createUnionTypeChecker(arrayOfTypeCheckers) {
    if (!Array.isArray(arrayOfTypeCheckers)) {
       true ? printWarning('Invalid argument supplied to oneOfType, expected an instance of array.') : undefined;
      return emptyFunctionThatReturnsNull;
    }

    for (var i = 0; i < arrayOfTypeCheckers.length; i++) {
      var checker = arrayOfTypeCheckers[i];
      if (typeof checker !== 'function') {
        printWarning(
          'Invalid argument supplied to oneOfType. Expected an array of check functions, but ' +
          'received ' + getPostfixForTypeWarning(checker) + ' at index ' + i + '.'
        );
        return emptyFunctionThatReturnsNull;
      }
    }

    function validate(props, propName, componentName, location, propFullName) {
      for (var i = 0; i < arrayOfTypeCheckers.length; i++) {
        var checker = arrayOfTypeCheckers[i];
        if (checker(props, propName, componentName, location, propFullName, ReactPropTypesSecret) == null) {
          return null;
        }
      }

      return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` supplied to ' + ('`' + componentName + '`.'));
    }
    return createChainableTypeChecker(validate);
  }

  function createNodeChecker() {
    function validate(props, propName, componentName, location, propFullName) {
      if (!isNode(props[propName])) {
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` supplied to ' + ('`' + componentName + '`, expected a ReactNode.'));
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createShapeTypeChecker(shapeTypes) {
    function validate(props, propName, componentName, location, propFullName) {
      var propValue = props[propName];
      var propType = getPropType(propValue);
      if (propType !== 'object') {
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type `' + propType + '` ' + ('supplied to `' + componentName + '`, expected `object`.'));
      }
      for (var key in shapeTypes) {
        var checker = shapeTypes[key];
        if (!checker) {
          continue;
        }
        var error = checker(propValue, key, componentName, location, propFullName + '.' + key, ReactPropTypesSecret);
        if (error) {
          return error;
        }
      }
      return null;
    }
    return createChainableTypeChecker(validate);
  }

  function createStrictShapeTypeChecker(shapeTypes) {
    function validate(props, propName, componentName, location, propFullName) {
      var propValue = props[propName];
      var propType = getPropType(propValue);
      if (propType !== 'object') {
        return new PropTypeError('Invalid ' + location + ' `' + propFullName + '` of type `' + propType + '` ' + ('supplied to `' + componentName + '`, expected `object`.'));
      }
      // We need to check all keys in case some are required but missing from
      // props.
      var allKeys = assign({}, props[propName], shapeTypes);
      for (var key in allKeys) {
        var checker = shapeTypes[key];
        if (!checker) {
          return new PropTypeError(
            'Invalid ' + location + ' `' + propFullName + '` key `' + key + '` supplied to `' + componentName + '`.' +
            '\nBad object: ' + JSON.stringify(props[propName], null, '  ') +
            '\nValid keys: ' +  JSON.stringify(Object.keys(shapeTypes), null, '  ')
          );
        }
        var error = checker(propValue, key, componentName, location, propFullName + '.' + key, ReactPropTypesSecret);
        if (error) {
          return error;
        }
      }
      return null;
    }

    return createChainableTypeChecker(validate);
  }

  function isNode(propValue) {
    switch (typeof propValue) {
      case 'number':
      case 'string':
      case 'undefined':
        return true;
      case 'boolean':
        return !propValue;
      case 'object':
        if (Array.isArray(propValue)) {
          return propValue.every(isNode);
        }
        if (propValue === null || isValidElement(propValue)) {
          return true;
        }

        var iteratorFn = getIteratorFn(propValue);
        if (iteratorFn) {
          var iterator = iteratorFn.call(propValue);
          var step;
          if (iteratorFn !== propValue.entries) {
            while (!(step = iterator.next()).done) {
              if (!isNode(step.value)) {
                return false;
              }
            }
          } else {
            // Iterator will provide entry [k,v] tuples rather than values.
            while (!(step = iterator.next()).done) {
              var entry = step.value;
              if (entry) {
                if (!isNode(entry[1])) {
                  return false;
                }
              }
            }
          }
        } else {
          return false;
        }

        return true;
      default:
        return false;
    }
  }

  function isSymbol(propType, propValue) {
    // Native Symbol.
    if (propType === 'symbol') {
      return true;
    }

    // falsy value can't be a Symbol
    if (!propValue) {
      return false;
    }

    // 19.4.3.5 Symbol.prototype[@@toStringTag] === 'Symbol'
    if (propValue['@@toStringTag'] === 'Symbol') {
      return true;
    }

    // Fallback for non-spec compliant Symbols which are polyfilled.
    if (typeof Symbol === 'function' && propValue instanceof Symbol) {
      return true;
    }

    return false;
  }

  // Equivalent of `typeof` but with special handling for array and regexp.
  function getPropType(propValue) {
    var propType = typeof propValue;
    if (Array.isArray(propValue)) {
      return 'array';
    }
    if (propValue instanceof RegExp) {
      // Old webkits (at least until Android 4.0) return 'function' rather than
      // 'object' for typeof a RegExp. We'll normalize this here so that /bla/
      // passes PropTypes.object.
      return 'object';
    }
    if (isSymbol(propType, propValue)) {
      return 'symbol';
    }
    return propType;
  }

  // This handles more types than `getPropType`. Only used for error messages.
  // See `createPrimitiveTypeChecker`.
  function getPreciseType(propValue) {
    if (typeof propValue === 'undefined' || propValue === null) {
      return '' + propValue;
    }
    var propType = getPropType(propValue);
    if (propType === 'object') {
      if (propValue instanceof Date) {
        return 'date';
      } else if (propValue instanceof RegExp) {
        return 'regexp';
      }
    }
    return propType;
  }

  // Returns a string that is postfixed to a warning about an invalid type.
  // For example, "undefined" or "of type array"
  function getPostfixForTypeWarning(value) {
    var type = getPreciseType(value);
    switch (type) {
      case 'array':
      case 'object':
        return 'an ' + type;
      case 'boolean':
      case 'date':
      case 'regexp':
        return 'a ' + type;
      default:
        return type;
    }
  }

  // Returns class name of the object, if any.
  function getClassName(propValue) {
    if (!propValue.constructor || !propValue.constructor.name) {
      return ANONYMOUS;
    }
    return propValue.constructor.name;
  }

  ReactPropTypes.checkPropTypes = checkPropTypes;
  ReactPropTypes.resetWarningCache = checkPropTypes.resetWarningCache;
  ReactPropTypes.PropTypes = ReactPropTypes;

  return ReactPropTypes;
};


/***/ }),
/* 21 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/*
object-assign
(c) Sindre Sorhus
@license MIT
*/


/* eslint-disable no-unused-vars */
var getOwnPropertySymbols = Object.getOwnPropertySymbols;
var hasOwnProperty = Object.prototype.hasOwnProperty;
var propIsEnumerable = Object.prototype.propertyIsEnumerable;

function toObject(val) {
	if (val === null || val === undefined) {
		throw new TypeError('Object.assign cannot be called with null or undefined');
	}

	return Object(val);
}

function shouldUseNative() {
	try {
		if (!Object.assign) {
			return false;
		}

		// Detect buggy property enumeration order in older V8 versions.

		// https://bugs.chromium.org/p/v8/issues/detail?id=4118
		var test1 = new String('abc');  // eslint-disable-line no-new-wrappers
		test1[5] = 'de';
		if (Object.getOwnPropertyNames(test1)[0] === '5') {
			return false;
		}

		// https://bugs.chromium.org/p/v8/issues/detail?id=3056
		var test2 = {};
		for (var i = 0; i < 10; i++) {
			test2['_' + String.fromCharCode(i)] = i;
		}
		var order2 = Object.getOwnPropertyNames(test2).map(function (n) {
			return test2[n];
		});
		if (order2.join('') !== '0123456789') {
			return false;
		}

		// https://bugs.chromium.org/p/v8/issues/detail?id=3056
		var test3 = {};
		'abcdefghijklmnopqrst'.split('').forEach(function (letter) {
			test3[letter] = letter;
		});
		if (Object.keys(Object.assign({}, test3)).join('') !==
				'abcdefghijklmnopqrst') {
			return false;
		}

		return true;
	} catch (err) {
		// We don't expect any of the above to throw, but better to be safe.
		return false;
	}
}

module.exports = shouldUseNative() ? Object.assign : function (target, source) {
	var from;
	var to = toObject(target);
	var symbols;

	for (var s = 1; s < arguments.length; s++) {
		from = Object(arguments[s]);

		for (var key in from) {
			if (hasOwnProperty.call(from, key)) {
				to[key] = from[key];
			}
		}

		if (getOwnPropertySymbols) {
			symbols = getOwnPropertySymbols(from);
			for (var i = 0; i < symbols.length; i++) {
				if (propIsEnumerable.call(from, symbols[i])) {
					to[symbols[i]] = from[symbols[i]];
				}
			}
		}
	}

	return to;
};


/***/ }),
/* 22 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/**
 * Copyright (c) 2013-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */



var printWarning = function() {};

if (true) {
  var ReactPropTypesSecret = __webpack_require__(11);
  var loggedTypeFailures = {};
  var has = Function.call.bind(Object.prototype.hasOwnProperty);

  printWarning = function(text) {
    var message = 'Warning: ' + text;
    if (typeof console !== 'undefined') {
      console.error(message);
    }
    try {
      // --- Welcome to debugging React ---
      // This error was thrown as a convenience so that you can use this stack
      // to find the callsite that caused this warning to fire.
      throw new Error(message);
    } catch (x) {}
  };
}

/**
 * Assert that the values match with the type specs.
 * Error messages are memorized and will only be shown once.
 *
 * @param {object} typeSpecs Map of name to a ReactPropType
 * @param {object} values Runtime values that need to be type-checked
 * @param {string} location e.g. "prop", "context", "child context"
 * @param {string} componentName Name of the component for error messages.
 * @param {?Function} getStack Returns the component stack.
 * @private
 */
function checkPropTypes(typeSpecs, values, location, componentName, getStack) {
  if (true) {
    for (var typeSpecName in typeSpecs) {
      if (has(typeSpecs, typeSpecName)) {
        var error;
        // Prop type validation may throw. In case they do, we don't want to
        // fail the render phase where it didn't fail before. So we log it.
        // After these have been cleaned up, we'll let them throw.
        try {
          // This is intentionally an invariant that gets caught. It's the same
          // behavior as without this statement except with a better message.
          if (typeof typeSpecs[typeSpecName] !== 'function') {
            var err = Error(
              (componentName || 'React class') + ': ' + location + ' type `' + typeSpecName + '` is invalid; ' +
              'it must be a function, usually from the `prop-types` package, but received `' + typeof typeSpecs[typeSpecName] + '`.'
            );
            err.name = 'Invariant Violation';
            throw err;
          }
          error = typeSpecs[typeSpecName](values, typeSpecName, componentName, location, null, ReactPropTypesSecret);
        } catch (ex) {
          error = ex;
        }
        if (error && !(error instanceof Error)) {
          printWarning(
            (componentName || 'React class') + ': type specification of ' +
            location + ' `' + typeSpecName + '` is invalid; the type checker ' +
            'function must return `null` or an `Error` but returned a ' + typeof error + '. ' +
            'You may have forgotten to pass an argument to the type checker ' +
            'creator (arrayOf, instanceOf, objectOf, oneOf, oneOfType, and ' +
            'shape all require an argument).'
          );
        }
        if (error instanceof Error && !(error.message in loggedTypeFailures)) {
          // Only monitor this failure once because there tends to be a lot of the
          // same error.
          loggedTypeFailures[error.message] = true;

          var stack = getStack ? getStack() : '';

          printWarning(
            'Failed ' + location + ' type: ' + error.message + (stack != null ? stack : '')
          );
        }
      }
    }
  }
}

/**
 * Resets warning cache when testing.
 *
 * @private
 */
checkPropTypes.resetWarningCache = function() {
  if (true) {
    loggedTypeFailures = {};
  }
}

module.exports = checkPropTypes;


/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(24);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.collapsible-component-profile {\n  display: block;\n  float: left;\n  width: 100%;\n  cursor: auto;\n}\n.collapsible-component-profile:not(:last-child) {\n  border-bottom: 1px solid #C8C8C8;\n}\n.collapsible-component-profile div.collapsible-profile {\n  width: 100%;\n  float: left;\n  position: relative;\n  padding: 15px 0 10px 0;\n  box-sizing: border-box;\n  cursor: auto;\n}\n.collapsible-component-profile div.collapsible-profile .row {\n  float: left;\n  width: 100%;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-settingsId {\n  display: none;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-settingsGroupId {\n  display: none;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-settingsGroupName {\n  width: 20%;\n  float: left;\n  padding-left: 15px;\n  word-break: break-all;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-authenticationType {\n  width: 15%;\n  float: left;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-contentPageUrl {\n  width: 40%;\n  float: left;\n  white-space: nowrap;\n  overflow: hidden;\n  text-overflow: ellipsis;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-primary {\n  width: 10%;\n  float: left;\n  text-align: center;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-primary .checkMarkIcon {\n  width: 16px;\n  height: 16px;\n  margin-left: auto;\n  margin-right: auto;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-primary .checkMarkIcon > svg {\n  fill: #4B4E4F;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons {\n  width: 8%;\n  margin-right: 15px;\n  float: right;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons:not(:last-child) {\n  float: left;\n  margin-right: 0px;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .delete-icon,\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .edit-icon {\n  margin-left: 5px;\n  float: right;\n  display: block;\n  cursor: pointer;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .delete-icon > svg,\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .edit-icon > svg {\n  width: 16px;\n  float: left;\n  height: 16px;\n  fill: #C8C8C8;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .delete-icon > svg:hover,\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .edit-icon > svg:hover {\n  fill: #4B4E4F;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .delete-icon-hidden {\n  display: none;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-actionButtons .edit-icon-active > svg {\n  width: 16px;\n  float: right;\n  height: 16px;\n  fill: #1E88C3;\n}\n.collapsible-component-profile div.collapsible-profile .row .item-row-wrapper {\n  padding: 0 5px 0 5px;\n}\n", ""]);



/***/ }),
/* 25 */
/***/ (function(module, exports) {


/**
 * When source maps are enabled, `style-loader` uses a link element with a data-uri to
 * embed the css on the page. This breaks all relative urls because now they are relative to a
 * bundle instead of the current page.
 *
 * One solution is to only use full urls, but that may be impossible.
 *
 * Instead, this function "fixes" the relative urls to be absolute according to the current page location.
 *
 * A rudimentary test suite is located at `test/fixUrls.js` and can be run via the `npm test` command.
 *
 */

module.exports = function (css) {
  // get current location
  var location = typeof window !== "undefined" && window.location;

  if (!location) {
    throw new Error("fixUrls requires window.location");
  }

	// blank or null?
	if (!css || typeof css !== "string") {
	  return css;
  }

  var baseUrl = location.protocol + "//" + location.host;
  var currentDir = baseUrl + location.pathname.replace(/\/[^\/]*$/, "/");

	// convert each url(...)
	/*
	This regular expression is just a way to recursively match brackets within
	a string.

	 /url\s*\(  = Match on the word "url" with any whitespace after it and then a parens
	   (  = Start a capturing group
	     (?:  = Start a non-capturing group
	         [^)(]  = Match anything that isn't a parentheses
	         |  = OR
	         \(  = Match a start parentheses
	             (?:  = Start another non-capturing groups
	                 [^)(]+  = Match anything that isn't a parentheses
	                 |  = OR
	                 \(  = Match a start parentheses
	                     [^)(]*  = Match anything that isn't a parentheses
	                 \)  = Match a end parentheses
	             )  = End Group
              *\) = Match anything and then a close parens
          )  = Close non-capturing group
          *  = Match anything
       )  = Close capturing group
	 \)  = Match a close parens

	 /gi  = Get all matches, not the first.  Be case insensitive.
	 */
	var fixedCss = css.replace(/url\s*\(((?:[^)(]|\((?:[^)(]+|\([^)(]*\))*\))*)\)/gi, function(fullMatch, origUrl) {
		// strip quotes (if they exist)
		var unquotedOrigUrl = origUrl
			.trim()
			.replace(/^"(.*)"$/, function(o, $1){ return $1; })
			.replace(/^'(.*)'$/, function(o, $1){ return $1; });

		// already a full url? no change
		if (/^(#|data:|http:\/\/|https:\/\/|file:\/\/\/|\s*$)/i.test(unquotedOrigUrl)) {
		  return fullMatch;
		}

		// convert the url to a full url
		var newUrl;

		if (unquotedOrigUrl.indexOf("//") === 0) {
		  	//TODO: should we add protocol?
			newUrl = unquotedOrigUrl;
		} else if (unquotedOrigUrl.indexOf("/") === 0) {
			// path should be relative to the base url
			newUrl = baseUrl + unquotedOrigUrl; // already starts with '/'
		} else {
			// path should be relative to current directory
			newUrl = currentDir + unquotedOrigUrl.replace(/^\.\//, ""); // Strip leading './'
		}

		// send back the fixed url(...)
		return "url(" + JSON.stringify(newUrl) + ")";
	});

	// send back the fixed css
	return fixedCss;
};


/***/ }),
/* 26 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(27);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 27 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.workspace-editor {\n  float: left;\n  margin: 20px 30px;\n  padding-bottom: 40px;\n}\n.workspace-editor .topMessage {\n  border: 1px solid #C8C8C8;\n  padding: 10px 20px;\n  background-color: #E5E7E6;\n  margin: 0 0 20px 0;\n}\n.workspace-editor .dnn-ui-common-input-group {\n  padding: 0 0 15px 0;\n}\n.workspace-editor .dnn-ui-common-input-group label {\n  font-weight: bolder;\n  float: left;\n}\n.workspace-editor .dnn-ui-common-input-group .dnn-label {\n  margin: 8px 0;\n}\n.workspace-editor .dnn-ui-common-input-group .dnn-dropdown,\n.workspace-editor .dnn-ui-common-input-group .dnn-dropdown-with-error {\n  width: 100% !important;\n  box-sizing: border-box;\n}\n.workspace-editor .dnn-ui-common-input-group .dnn-single-line-input-with-error {\n  width: 100% !important;\n  padding-bottom: 15px;\n}\n.workspace-editor .dnn-grid-system .left-column {\n  padding-right: 30px;\n  border-right: 1px solid #C8C8C8;\n}\n.workspace-editor .dnn-grid-system .right-column {\n  padding-left: 30px;\n  border-left: 0 !important;\n}\n.workspace-editor .editor-buttons-box {\n  width: 100%;\n  text-align: center;\n  float: left;\n  margin: 30px 0 0 0;\n}\n.workspace-editor .editor-buttons-box .dnn-ui-common-button {\n  margin: 5px;\n}\n.workspace-editor .editor-buttons-box .edit-icon {\n  margin: 0px 10px 20px 10px;\n  float: right;\n}\n.workspace-editor .editor-buttons-box .edit-icon svg {\n  width: 16px;\n  float: left;\n  height: 16px;\n}\n", ""]);



/***/ }),
/* 28 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(29);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 29 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.dnn-pbiEmbedded-workspaces .workspaces-items {\n  width: 738px;\n  float: left;\n  box-sizing: border-box;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .workspaces-items-grid {\n  border: solid 1px #C8C8C8;\n  float: left;\n  width: 100%;\n  margin-bottom: 40px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-row {\n  border-bottom: 1px solid #C8C8C8;\n  padding: 10px 0 10px 0;\n  width: 100%;\n  float: left;\n  overflow: hidden;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-SettingsId {\n  display: none;\n  width: 0%;\n  float: left;\n  font-weight: bolder;\n  padding-left: 15px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-SettingsGroupId {\n  display: none;\n  width: 0%;\n  float: left;\n  font-weight: bolder;\n  padding-left: 15px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-SettingsGroupName {\n  width: 20%;\n  float: left;\n  font-weight: bolder;\n  padding-left: 15px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-AuthenticationType {\n  width: 15%;\n  float: left;\n  font-weight: bolder;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .header-ContentPageUrl {\n  width: 40%;\n  float: left;\n  font-weight: bolder;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .collapsible-component .collapsible-header {\n  text-align: right;\n  text-transform: none;\n  padding-right: 40px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .collapsible-component .collapsible-header .collapse-icon {\n  display: none;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .collapsible-component .collapsible-header .collapse-icon.collapsed {\n  display: none;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow {\n  text-align: right;\n  width: 100%;\n  float: right;\n  margin: 0 0 15px 0;\n  font-weight: bolder;\n  border-bottom: 1px solid #C8C8C8;\n  overflow: hidden;\n  height: 25px;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .sectionTitle {\n  font-weight: bolder;\n  float: left;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox {\n  width: auto;\n  float: right;\n  color: #4B4E4F;\n  cursor: pointer;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox .add-icon {\n  margin-left: 20px;\n  margin-right: 5px;\n  float: left;\n  cursor: pointer;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox .add-icon svg {\n  width: 16px;\n  float: left;\n  height: 16px;\n  fill: #4B4E4F;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox-active {\n  width: auto;\n  float: right;\n  color: #1E88C3;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox-active .add-icon {\n  margin-left: 20px;\n  margin-right: 5px;\n  float: left;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .AddItemRow .AddItemBox-active .add-icon svg {\n  width: 16px;\n  float: left;\n  height: 16px;\n  fill: #1E88C3;\n}\n.dnn-pbiEmbedded-workspaces .workspaces-items .profile-item {\n  color: #4B4E4F;\n}\n", ""]);



/***/ }),
/* 30 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(31);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 31 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Imports
var urlEscape = __webpack_require__(32);
var ___CSS_LOADER_URL___0___ = urlEscape(__webpack_require__(33));

// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.dnn-pbiembedded-generalSettings {\n  box-sizing: border-box;\n  padding: 35px 25px;\n}\n.dnn-pbiembedded-generalSettings * {\n  box-sizing: border-box;\n}\n.dnn-pbiembedded-generalSettings h1 {\n  margin-bottom: 15px;\n  text-transform: uppercase;\n}\n.dnn-pbiembedded-generalSettings .panel-description {\n  margin-bottom: 30px;\n}\n.dnn-pbiembedded-generalSettings .logo {\n  float: right;\n  background-image: url(" + ___CSS_LOADER_URL___0___ + ");\n  background-repeat: no-repeat;\n  background-size: 120px;\n  width: 120px;\n  height: 120px;\n}\n.dnn-pbiembedded-generalSettings p {\n  margin-bottom: 20px;\n}\n.dnn-pbiembedded-generalSettings .dnn-switch-container {\n  width: 90%;\n}\n.dnn-pbiembedded-generalSettings .directory-section {\n  margin-top: 10px;\n}\n.dnn-pbiembedded-generalSettings .editor-row {\n  display: inline-block;\n  width: 90%;\n}\n.dnn-pbiembedded-generalSettings .input-full-row {\n  width: 95%;\n}\n.dnn-pbiembedded-generalSettings .dnn-single-line-input-with-error {\n  width: 100%;\n  padding-bottom: 15px;\n}\n.dnn-pbiembedded-generalSettings .dnn-dropdown-with-error {\n  width: 100%;\n  padding-bottom: 15px;\n}\n.dnn-pbiembedded-generalSettings .dnn-ui-common-single-line-input.small {\n  margin-bottom: 0px!important;\n}\n.dnn-pbiembedded-generalSettings .buttons-box {\n  margin-top: 50px;\n  margin-bottom: 30px;\n}\n.dnn-pbiembedded-generalSettings .buttons-box button.dnn-ui-common-button[role=primary] {\n  margin-left: 10px;\n}\n.dnn-pbiembedded-generalSettings .dnn-ui-common-input-group .sectionLabel {\n  border-top: 1px solid #C8C8C8;\n  padding-top: 20px;\n  margin-top: 20px;\n}\n", ""]);



/***/ }),
/* 32 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


module.exports = function escape(url, needQuotes) {
  if (typeof url !== 'string') {
    return url;
  } // If url is already wrapped in quotes, remove them


  if (/^['"].*['"]$/.test(url)) {
    url = url.slice(1, -1);
  } // Should url be wrapped?
  // See https://drafts.csswg.org/css-values-3/#urls


  if (/["'() \t\n]/.test(url) || needQuotes) {
    return '"' + url.replace(/"/g, '\\"').replace(/\n/g, '\\n') + '"';
  }

  return url;
};

/***/ }),
/* 33 */
/***/ (function(module, exports) {

module.exports = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAJYAAACWCAYAAAA8AXHiAAAACXBIWXMAAC4jAAAuIwF4pT92AAAGWWlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDg4LCAyMDIwLzA3LzEwLTIyOjA2OjUzICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuMCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIwLTEwLTI0VDE1OjMzOjA3KzAxOjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMC0xMC0yNFQxNTozNTo1MSswMTowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMC0xMC0yNFQxNTozNTo1MSswMTowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo5YjA1MjdiOC05ZGMzLWUwNGUtOTdmNS1jNGU2ODY5NTI3YjEiIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDplMzFjNTdmZi1jYjQ4LWYyNDktOGFhZC1iNzI5Y2JhOGM1ZjMiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDpiNzM4MjA0Yy0wMjkzLWExNDUtYmUzNi05MGM1NGIwMWM3ODYiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOmI3MzgyMDRjLTAyOTMtYTE0NS1iZTM2LTkwYzU0YjAxYzc4NiIgc3RFdnQ6d2hlbj0iMjAyMC0xMC0yNFQxNTozMzowNyswMTowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIyLjAgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjb252ZXJ0ZWQiIHN0RXZ0OnBhcmFtZXRlcnM9ImZyb20gYXBwbGljYXRpb24vdm5kLmFkb2JlLnBob3Rvc2hvcCB0byBpbWFnZS9wbmciLz4gPHJkZjpsaSBzdEV2dDphY3Rpb249InNhdmVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjliMDUyN2I4LTlkYzMtZTA0ZS05N2Y1LWM0ZTY4Njk1MjdiMSIgc3RFdnQ6d2hlbj0iMjAyMC0xMC0yNFQxNTozNTo1MSswMTowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIyLjAgKFdpbmRvd3MpIiBzdEV2dDpjaGFuZ2VkPSIvIi8+IDwvcmRmOlNlcT4gPC94bXBNTTpIaXN0b3J5PiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PsI4xhEAABlWSURBVHic7V1NkxxHWn6yu0cejywhy15YLFkYhwgvHNggghOn5WfAHrjD+oMw/tglsCTzA7hx4cDKu8GFIAhuewAuG7bX+EIscNxAkiOwZMtG+GN6ema6Xg5VWZUf75uZVZVZ3SPNqxh1Zb75VZVPP09mVlW2IiLkMKVUlnLG2l/80eN4+9Wz+OzWMZYHwGw2SbX/CuD3AXwJ4XJS+59gFAwGIhtXrBsZP3UHCwCPA/i7K3/4v9+NlJRkixyFbJP95c0lFjPgrZf3cP/2GgeHwKw85s8BAKj5NIy8A8ZioBoDqFj5tm83obQke+iABQDX/nYJIuDay3v47E6F5QGVZq61G9GXodio6QCV3R5KYAHA9R8uAQDXXjmLz28dY38C5srCTpH8fSVvSjCZ9tACC6jBpVDLYnV7jYNVuTHXxhkqVn4kf6ahdmsPNbAA4NoPlyAA117aw/1pZNG2EeMnoDxD5QaUtoceWEDNXETA9VfO4v6tYyynGNBvz4B8WPkj7ZEAFgDcuLnETAFvvbSHT0vK4hYDKlpHRsA9MsACHFm8XWG5yiSLWz7DmwpMpj1SwAIMWXy5XkTNOVuceFEz3p4NAErbIwcsoJZFBeDai3uo7uRZRD0xDCXkz42zRxJYAHD95hJQNbg+LSyLrWuTSwZDbzUNtEcWWIAti5+OmS1u85LBSDkdao80sIBGFqlmrk8yyaK2bRxDlWIo1x55YAHA9XfyyeI2jp/a6NNbOtPb9Zv1UsT1F8/i09v9ZfFEj58IoMyoOwWWYTduLgECrn9vD/dyyuK2jqFIf+SnslNgOXajkcXrf7KHT+6MnC1OvKi5SYZy7RRYjN24Wc8Wb7x0Fp/c7vkk6jYPyEOAyoyzU2AJ9vY79SJqL1ncBEMllD0loLSdAitgN96pHxbsI4tT3BTeRoZy7RRYEbvxTiOLLzayKDDXZLdcxgzIY+2KFN3HToGVYG//aAnVDOjvhmTxhDJUiTWuU2AlmimL9wYsom5kySAGppTyB9opsHrYjXeWoKqWxXsJi6iPCjtxdgqsnvb2j9NkcWOLmmNmkBkBdwqsAXbjR80i6h/v4Z4xW9zGJYOpAaXtFFgDrZXF753Fx7eOcXCUPqDvNb4pJXnEHmazU2CNsLd/vERFwA/+YBf3vyBUleHc5jFUYVABwJRv2JUytcE//NU/HOAXH1d4YleBqJZDqtAdN39V88f5rL8KqCpCxekqIQhYCvjbvGQd+mky2aPMWC04RtgaAKiC0sDQ5i6YBisquWQQY6fT5QbLpH6KAaXIa6om47Rx7X9GGIC325MjeaabDGl182UZQxVcdjipwOpjk23cZXV2iyQukQYhgZSTpDRDxfJnsocBWBxwFHOsUOCSErqxEyohgXFcmRFNi8hopJncikte3wr7WdfpcoNlLqDMMRMZYUJ4PDWa0aoKSg/OzRKVyR6aoVDLmtJx5CBedbJXofOLrRw5hjrdFCRumi+sYQq6fiPw3SPFJxkRsG5mce1Mzppy8T1HTZVsg6jLJU7bRwDqdFMQ2TgG0sCa4LJ1RkQ42F8dHewDB/tatshiIUuXlarlr6owmyuceewM5osFyASlWb4yyhg5fprylfuTDCxt61+7tPfUP/3N772lFC4sD9YH9XoQNX1RX7FmnUd5V7cJk8swypQyArlDNAIqUuv5rKqeu3jwG2fmFR6rlNB7Jmjqcqr1MY729/Hg3n0cfrGPJ87tYjGfWXUoprhtB5S2kwosyx7fWzz17e9880XMlcLXx00sITz6dRmCB5yYnrryj/4PqNaAUnweuyhqBlkKWK9x/vKXuP1fv8CXn9/FhQu7LaO5RgljLM/VA1C58XWSgdVe6uq4WlX/s7+czdXeclkDq8NLT0AZeTrPMBB6LMikf+LCE/j2d34H//Huz/HRf3+EZ595AkQEzV2kmHFWRnYqNW44qbd0lPPZmAEId8XS8AcBQmSMvZujNoL4PFwbEkG42t/H8eoQv/7b38KZc0/iwYMDrNd1Lm71IidDeckzouykAguwB/CKXFBZFgFIX0B5S+3U/nXRQh6nTQoKq+UK58/t4Mq3LuPug2OsjsmeyzLFWWfHfYecUxeCZtOz2kmXwo65iJQeodfmspMXmFTyvDYY4z6lFFarQ5x78hyq+S6Wq2Ps7e6gImBGAM3gc/NYySulgY2dVGBZTxgY3+/G0gDiAao9ZAAkAJTY9BBB6Javj6r1GvP5DBV2cHRYv+pPVT3GcokvaIzkhSPK2EkFlmsNyAwJA1CKoWxXYh4WUF1bFTXrW408dmlVWOq4JiSkcX258XbSgdUJBFH99eYYZDSg6jTeskEwTxqg2lhVgUD181gNkCoj55xp0Sh24kYKGe0kA0tBTz4IsxBI7JjS46gIoKS6qWurfjAQFYAZ/DWsLWQo17IB6/HdHSwPjnIVFzPnYQBSFnyiDOUFigGqDsljNC9Pw1htEvdYLmU8Q2VEW7blhv/86Wt44eo3chUXM+deYXOYvGxAVnpuGaDvskFLNVbNVN8DlEBl5anPoYLxGDMAoq4M89vkFSmBwr0EgTQ5LRuwnv/d38RP/+VVXL1yMVeRMTPA1XW2Bag2grlywlpXb0C1ZTGyJ/U++Xnac+CYqrHKjYoBYgOA0pYNWIcffYRvXPkV/Nv738fzly/kKjbZbK4wIxhAjWKoJk1LfjagyAVIMqBqptXPYOmqTfaSKYstLhlQ5EeNtnwr7/M5Vnc+xpPPPI0PP/hzXH32yWxFC2ZehxmUcTV7ME5/ydPxLYxqyRsMKLskRWQDioxHZjzKYouTr5bbJPdSZURW1ls6alGD6+IzF/H+e2/i6pWi4LLHWe2Ntb6AArwrKg7M7bJbhrSi0wFVh5wyOXCEOlzjPZDEO73UskdY9nuFajHH4Z27ePry0yVl0X3Qz+3dHgxFdvrgwNwqaSRDaVDBykOpP9ruEjTjlySviPY5VuYm9GKO1Z27jSz+YApZdBZKM0ieBSgyS8oieVb5ZJdhYdwoStnF8SaAiTv1klbs6QZbFt8oLYsBgAiAgpsegACQNEAZ+a069BHT42x6puerzpUKKK/0GKAyg67oYzOWLL73ZrnZoncFyfjWCwzF5BnHUOSURYZUuRQSZjWzKFPu2H5PYagQYJw6cln557G0LF56Gh/+rKQs2oDiv8IcQ40FlOHX4CCToXQeCubpCjaSx0+XD6aCKVbHCJvkQb9WFi9dxPvvvZ5fFo2F9+bI9ictHYwAVEzy2HUwoQ6/9V7WMYACBZJkRNlkT5CqxRyHt7UsvpFVFm3JI9dhpoTNUGZsuLODgCKjDMudwlAmSwV6PROgvGTMZcth0z6avJhjdfsunrz0FD784Pu4+uyFMvVEAdWToVzGMQYlHaAirCbUUSdv7gcC/uu2EqBSAEFWUz1fMR3EBp55Vw246tliTlnUVzEOKO3hB/FWAr6OtuQEQFna47aBAa1OVvlR5DzTIZpbLeNLjB5sG3mZwpbF1/H85V8aWSIHEJmh/A5NZ5zeDGXRi8NQXJuNjQIsUtJgy8xQVvMzImtzb+ks5ljdvtfI4puZZothQPGdbaTPCSizPW1yCdBGu5SZC05APm1xeMbkb2vNTVOGbfT1L7WYNbL4FN5/77WRsugDqo4Ny1FQ8jhAwa+D1x6yj4KLsd3BFIDKrnuMbfi9QpVPFhlAUYeMNhbm4RCGCgzkjYRGFoGlQr0sdbrxruG2Akrbdryw2s4WL+LDD94YNVu0ntrkvv5DAcWmJxiJrJy9AWWtWDI3oglAJWCDwDqspocAlZKmp20HsGDOFofJYteZXqyZAHABArL7OyiTBjjsREbygYCK9GwQUFJ0DCju5Ykk72NbAyzAnS2+1k8Wh87yyOKZLj2bx4yyoTiYodzSjCUF8bZLCFB9GCoh6VDbKmABcBZR+8iic0VjgIIJqFh63bt2HWMBRca/NlkFHkxMUVZ0KpjIPQvvtLLY9gEL7iLqq/1kUQKIIWE5ZnlxQNl5unp0C8z6jfziOQnRKYBwqxJ8OW0rgQVoWbxXy+K7fxaXRSLnhTzAZKi6z5kOZW8QwwHHEIYKsZRbfxqgzA1okgHFMRRTdm7bWmABABazehH18lP48Gevx2WxnUz5ktd2p9Qz3oAmk+SRKXhk+6z0VVc+0+kE4w2enpLH+aRsuWy7gYVGFm/drR+5efdVXL1yAQAOAHwN4CsAANEDqlZE1Qq0XoHWh6DqEFV1CKpWoOqwjlvrcB0HOrI7tC4MSYDShywKApLHMiTBfEXS8iq5GrcY97vj+QNZcyMr594N5wF8E8BxLGFfU4s5Dm/dw9PP/TL9809eVK/86d8/RkrNdnbm6rMHR1+88NxjL9Ds3IyUwmyn24KtIzDuiq1B6yPQ8RKgNdRiF91WEFwed6DCfe3JSBJLT06UTScag6pCeLNwtxrBJ7oKyWFOYH0XwF/bLSXrg/WFwma+xQyrW/fw7KXz+MefvIQav/U3nQ6PcHTvS1Qg7HhPAHB16p+EOEZ19CXWX3+E9Ve3gdkZQO3weaSecAA4DFCGS++UzO4TKWYTfayrEJhMywgsc0+UkYBqP2yfWsxw/MU+Zl/td3H6pu1sDqvTvB2MzfIa6ZntYr5zHvO9Szg+cxGHn/+8/lWImQZXOqiSABUqq3JkU7KSDLWdUuis9gXBJPjFs7fT6h+cJNNH3DMlEUATQOsVoObYufBbAK2x+vzfMZvNw+0ZLXmmqzmL2A+yUH3Ye8PbiH/7pTCH3IlpHcwGWC2pHC/JGtXxF1icfx7r/Y+xPrgPNd/1ExYAlO0nv7nNGMvgT/4cOFcPQOXGV8ZZoXnq7mUQwuz9B2L+9JFx8dmFHLd+p1wrymlvdQQ128F871cBOrbzGssD/lqYdG5cu7SLaXdFsH5jzvgOtKkTniBtMb5BUAF5pTASN4xlWlj1ZDa/zlhagOgYaudsPYgnezoWZyggNpNkAaWjlBE2vgPm0lwSoIIJxGAgcpgV2Coyo9xlAZMZiICf1gBmUJjVIFEqAVBhdqo/UvI0jCi5Qi0fA6gSdIWijKWvaiKTtUchZjPjxrGTV07bt/UkgNx8oSdAuXpiDMWkbynK/dVFqfVbxFCuZWasVLkjx5MKpkC5SYCSpFkfNKyhezTUnpyAMuoGQX5pQq9xbSFDuZZxVhhhEuc4zk5D5S4hbyyfbo9yfGI+7R4CKD+LGyREADWSnVhRGWnlpdA5zguooewUymtqUkJeFlCQpbM9dFhbvxIm/QasZCMZqgSogFJS6B33kbue0tNb7kL5+jFOf4by6+0e8jNmoCEVFprfB1ClwGRaAcbaNDs1YaEj420g48ozC0cZJI+8c0Mru+19wtB3jw8GItGVLToDvgFWRAqHAyr2DR8qd7FeMvJ6j8h0h/xqudRWtyydQgChvvvsZhEWRbMx1LZLYRxMZtwm5Y45btvrtq/74aT0toI9/+CvU5iA5n7ehPu+MdW6tglAadvYrNAKj2anoXUGwD8IUAmSJ8mk0o/yAO4daQdb2dmpBMYmnxVuRu5idepw/UekV7KYjZm5dg1hKKkNEjsFIxvXGFBlRlfBWzoxuQsByA2XkEkuLbVH8tOnYwEllGfchGYZSgLGWIYqJIlFZ4VWOKvcJYJysFTq/CGmzQAoM0oRCJXdZB3g3rjfUkBpKySF2yB3iWktH8GamSknbU6GspIRyH2ZgmmiUmgfchSNyZcMqIxgKyOFWcHUN29fdkIHgOjbOKY3JM+QJdQBVGuVW57TErd5garZ6AigchNYgVkhUF7uxjBiEw4CkSDdhO7PUPAQ4T8oCHgLVgr1gxZCkVJ8X3YqpYiT3dIZDigzLlFmQuVEAEWgZhGcvBcyqOdA3qUCMs/Da14DaOVIImcxhoqhpQBDuZYRWPqJywhzsV+pEJi4vCPkjvF1/5vtUc1RRKZ7AQqyRGqG1LdzYqTLRY9lqIxoK8RYU7NTnzrrMLlpnTC/AW6oPU1cX0CR6WcQxYHMjYoAaiowmVZeCpMA1acDU+WO94UBVYOpIqoXR91ZYahNEqBE2YNzbYRd/Jxfvq5cv2QTM5RrmaWwsd5y54KJS59L7gL5pFlhqOyhDOUFmvoFdiKgvtMjMJjQJDGNH5UXZQXeK3QDCYAaDSY+bZCdgnUaVJEdUC4DNm0lFTz70YASnP4sN49tdlbYHpaQu1A+qc5+DGXW5vtiZQEgV5qZr0JJQAX8Yy3zyvsJkLtQfao5h4oawjI3BDVTJjCUdeizlB3TMaR1FQ3i9GzLGMq18rd0ssldCEyCf0ydXhkjJa/J4wPKODR2H+SyKzSbFoZsw4DSNo0UbkTuQuwptIcI9RMGugzuTWiugyKgigEKQHDfIk2kcorxgMoMunJSyF7kVDBx/kxyJ+bTB1W72Zk3UxowMA8DyvTOmPz1R6UC/c4Ro9eucN4SRJYXWEXlbhgQRR9JaeuOITQP+klvcgfy9wOUDlddtCbPJugNszKwU2lVLHQTuo0Ujo1wMbnrCwiDcRWgdO+KbWbqGAQou+pop5dkqIxoK7wpSH9QkJmOKzeL3IXS6s1ujS2FxgCq8YfbVAfEPf3ITdvl4Krj8k8BJtMmuFdohIuxk1BnkJ0kHxOfjaHggaw785ncLNV9kM4zVvIKa2GZWzqtxVkmr9zxTMAfc76mTv2qu/W2jAOIkY/QWOys6+Sk0bBKUXzTW6Nqkf0msAJSCKTJXQhMnD+n3DHHbecaG5y6G+QS1/KRgErQsUpXXUjytnxTEGA8oPqwk5mP8ycAKppX3y8cIHm9AUVGD6sG53WY3QDaaUpfdiq9f0PGWaEshTygQjJpxvWRu1A5zrFXL0cJ5ODDZTChfRKgAKZH9fkoI30FeRvlQFVMsaxrAjksJoXj2InzZ5K7aD4NJoK9glRA8hiQBdXReCCrL6CSwJQRcNmlMK/cpUoWFx4CxPot6IoISr/y7iUbCyg/fZJ1mOd9UraJAaUt46Ygw2aF/rGbz02TCMwxzNaG05/J0nGDQUWAOcaysgwAFMzixATJRfW2jcwK7U8un5uWSz9G7oR8euqlGlDlkrwYoNDuYG9ncdWYOwXX1ZOhCpAVgIlnhfanE5/EMIJ/KDtx+QjNC0eE2B6koyTPAJSdh9mSmyvedY9hp22WwtCs0D82wlnlLhGI7SHXJv0n7UFKTrDPeXX1eoBqPvpKXh+GGiKnQ62cFFqfjm+TchcFrXHsvFxhN3u45LHp2xVQY9KgfX03BeGqFnylbAP3CmNpBV9OuYuVY92EdiVPSi+UHQOUix7zKVLGth1Q2qZ5EzoJFGZcBrkbAiivR0fO8lzZtPJwZfE9TxgJKDHScGcGXTnG6s1OznFWuUsAtO69RpLqr0n6zNCtP85QOqi/CL7eibtvhqoWfGL2QiyW/2WKrHJXmJ2MfGaNBHQbg4h5uLb3ABQgSihRv5cm+gLqZN3SSZK7EMuYB30A1YMFmbweELQkBV7/klinN0s55aU+wSCUOh5QGQE3wVaRgm80EHuyk5HPb6HLV0Z8kKGY0sR9wlxf5+82VRN6tmnWkJ/uZavtkXeoTfRDmEPZKQZEN5wmd+n5+F+mEMuSwBFiKLZ+p9ieP90rVhvInxtf0z3zHmUZIW9U7gTfUEDpvO7z7kBE8jICyq1WH6RsCsJVK5Sb0IrBVlYKe7ETlzeX3IXyNXEEU5NggQxCmRJAhgCKeQtap2p3U8oIKDZpRoSVkcJRctcDfDrcdsRwmfQ+mz2DZMljyuwLKCt9V5+FodKAKkFXyC2FHjASpW+U3AnyNKjOjqVIP+Hg1cfkE27v2NEpIGziTMkLPEi6jYDSNsFv6bhxkW99EblLBbAOkfFzJwUkj22b8R690OlD9nmfGlDaNjQrHMpOOeQuFHbKkr4soyTP8SdoHjVFpO7zziaLsFtuvG14VmjGZZa7XoAyj5tti/WuM1ZSOS+/DsX1mMTY/o5+bRGJgGKjEuSyBImV3R9ra+QuAdBk/pG98t4bUIE8zjVpW+FSkg4qZpi1hQzlWsFZ4XB28r2F2IlcnyNHUclj6ugJKO85d2KaJlUV8QebkZpvoBWeFbrhbZE7MHkZUClnVgj0H0dJgGKBLRj57r6gEt2FqKvgNkYp7FQfl5G7EJhC7SMHVJkBJV4n5TfZITPxFCTbAKC0beiHMGM3gqV8nL+v3En5tJmAovKAItPP65X3o/aZ2akExjbyQ5jj5U4AX6BOOR9XDoGIuQENBBhHH5Lj4Xpaal9tStV/bGulU+AxGczXi/16WtnfhDYu9PRyl8JOtul7crUSmlqEBIaqA3YwjdVsP6HCzPJwv7wSbIZUPBddSBIL/hDmULmLsZEbHiJ3kpksFVlqSJa8WBubPM0vU3jJhuzzHgNUITCZVmBz28i2iVnkzpUfwTfGQozJSh76MxSZZdSwUgBmZOwIzrZr+xjKtXx7N4TGHslgioXHy12SWQukMojHAsqDpkdZdQuUk8RvLxe5GUBp29Cs0PXH5C+n3KUaQWpvTViBNg8GFHV4Vqj3m2+WIbzathBMpuWUQuE3Z3n5CKZ1j728pa9YM7jJJXlGHhZUxoFSHUtJV5QtPuKf2nICa9Uc7ANYb73chUxXo3JLXhMKoEBY5CjNUAsAjwM4GFyCY/8Pkc0G4HRzTsQAAAAASUVORK5CYII="

/***/ }),
/* 34 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(35);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 35 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.dnn-pbiembedded-permissions {\n  box-sizing: border-box;\n  padding: 35px 25px;\n}\n.dnn-pbiembedded-permissions * {\n  box-sizing: border-box;\n}\n.dnn-pbiembedded-permissions h1 {\n  margin-top: 10px;\n  margin-bottom: 10px;\n  text-transform: uppercase;\n}\n.dnn-pbiembedded-permissions p {\n  margin-bottom: 20px;\n}\n.dnn-pbiembedded-permissions .dnn-label label {\n  font-weight: bold;\n}\n.dnn-pbiembedded-permissions .dnn-switch-container {\n  width: 90%;\n}\n.dnn-pbiembedded-permissions .editor-row {\n  display: inline-block;\n  width: 90%;\n}\n.dnn-pbiembedded-permissions .dnn-single-line-input-with-error {\n  width: 100%;\n  padding-bottom: 15px;\n}\n.dnn-pbiembedded-permissions .dnn-dropdown-with-error {\n  width: 100%;\n  padding-bottom: 15px;\n}\n.dnn-pbiembedded-permissions .dnn-ui-common-single-line-input.small {\n  margin-bottom: 0px!important;\n}\n.dnn-pbiembedded-permissions .buttons-box {\n  margin-top: 50px;\n  margin-bottom: 30px;\n}\n.dnn-pbiembedded-permissions .buttons-box button.dnn-ui-common-button[role=primary] {\n  margin-left: 10px;\n}\n.dnn-pbiembedded-permissions .warning-container {\n  width: 100%;\n  float: left;\n  margin: 10px 0 15px 0;\n  font-weight: bolder;\n  color: #EA2134;\n}\n.dnn-pbiembedded-permissions .warning-container .collapsible-content {\n  margin-top: 5px;\n}\n.dnn-pbiembedded-permissions .warning-container .collapsible-content > div {\n  border: solid 1px;\n}\n.dnn-pbiembedded-permissions .warning-container .warning-icon > svg {\n  width: 17px;\n  float: left;\n  height: 17px;\n  margin: 0 10px 0 0;\n}\n.dnn-pbiembedded-permissions .warning-container .warning-msg {\n  margin-left: 30px;\n}\n.dnn-pbiembedded-permissions h1.spacer {\n  margin-top: 25px;\n}\n", ""]);



/***/ }),
/* 36 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(37);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 37 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "/* START EVOQ COLOR PALETTE */\n/* END EVOQ COLOR PALETTE */\n/* START ATTENTION COLORS */\n/* END ATTENTION COLORS */\n/* SVG HOVER STATES */\nsvg {\n  fill: #C8C8C8;\n}\nsvg:hover {\n  fill: #6F7273;\n}\nsvg:active {\n  fill: #1E88C3;\n}\n/* END SVG HOVER STATES */\n.dnn-pbiembedded-objectlist {\n  border: 1px solid #ccc;\n  width: 100%;\n  margin-top: 5px;\n  padding: 15px;\n  display: inline-flex;\n}\n.dnn-pbiembedded-objectlist .listview {\n  width: 250px;\n}\n.dnn-pbiembedded-objectlist .listview label {\n  font-weight: bold;\n  text-transform: uppercase;\n  padding-bottom: 10px;\n  display: block;\n}\n.dnn-pbiembedded-objectlist .listview .none {\n  font-style: italic;\n  cursor: default;\n}\n.dnn-pbiembedded-objectlist .listview .workspace {\n  margin-bottom: 15px;\n}\n.dnn-pbiembedded-objectlist .listview .dashboards {\n  margin-bottom: 15px;\n}\n.dnn-pbiembedded-objectlist .listview li {\n  display: block;\n  padding-left: 15px;\n}\n.dnn-pbiembedded-objectlist .listview li .pbiObject:hover {\n  color: #1E88C3;\n}\n.dnn-pbiembedded-objectlist .listview li .pbiObject.selected {\n  font-weight: bold;\n  color: #1E88C3;\n}\n.dnn-pbiembedded-objectlist .permissions {\n  width: 450px;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-caption > .dnn-label {\n  line-height: inherit;\n  margin-top: 0px;\n}\n.dnn-pbiembedded-objectlist .permissions .user-permissions-grid {\n  margin-top: 30px;\n}\n.dnn-pbiembedded-objectlist .permissions .dnn-suggestion span .rw-combobox {\n  width: 175px;\n}\n.dnn-pbiembedded-objectlist .permissions .dnn-suggestion span .add-button {\n  padding-top: 7px;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-caption {\n  border-bottom: none;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-caption .dnn-grid-cell {\n  width: 50%!important;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-header,\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-row {\n  min-height: 30px!important;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-header > div,\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-row > div {\n  width: 50%!important;\n  min-height: 30px!important;\n  height: 30px!important;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-header > div + div,\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-row > div + div {\n  width: 20%!important;\n  min-height: 30px!important;\n  height: 30px!important;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-header > div + div + div,\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-row > div + div + div {\n  width: 30%!important;\n  min-height: 30px!important;\n  height: 30px!important;\n}\n.dnn-pbiembedded-objectlist .permissions .permissions-grid .grid-header {\n  margin-top: 0px;\n}\n.dnn-pbiembedded-objectlist .permissions .dnn-grid-cell .left,\n.dnn-pbiembedded-objectlist .permissions .dnn-grid-cell .right {\n  width: 50%!important;\n}\n", ""]);



/***/ }),
/* 38 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(39);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(7)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 39 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(6)(false);
// Module
exports.push([module.i, "#PBIEmbeddedAppContainer .dnn-persona-bar-page-body .persona-bar-page-body .dnn-switch-container .dnn-switch.place-left {\n  margin-left: 0px;\n}\n#PBIEmbeddedAppContainer .dnn-persona-bar-page-body .persona-bar-page-body button.dnn-ui-common-button[role=primary] {\n  margin-left: 10px;\n}\n", ""]);



/***/ }),
/* 40 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
// ESM COMPAT FLAG
__webpack_require__.r(__webpack_exports__);

// EXTERNAL MODULE: external "window.dnn.nodeModules.React"
var external_window_dnn_nodeModules_React_ = __webpack_require__(0);
var external_window_dnn_nodeModules_React_default = /*#__PURE__*/__webpack_require__.n(external_window_dnn_nodeModules_React_);

// EXTERNAL MODULE: ./node_modules/prop-types/index.js
var prop_types = __webpack_require__(1);
var prop_types_default = /*#__PURE__*/__webpack_require__.n(prop_types);

// EXTERNAL MODULE: external "window.dnn.nodeModules.ReactRedux"
var external_window_dnn_nodeModules_ReactRedux_ = __webpack_require__(5);

// EXTERNAL MODULE: external "window.dnn.nodeModules.CommonComponents"
var external_window_dnn_nodeModules_CommonComponents_ = __webpack_require__(2);

// EXTERNAL MODULE: ./src/constants/actionTypes/index.js + 1 modules
var actionTypes = __webpack_require__(4);

// EXTERNAL MODULE: ./src/utils/index.jsx
var utils = __webpack_require__(3);

// CONCATENATED MODULE: ./src/services/applicationService.js
function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }



var applicationService_ApplicationService =
/*#__PURE__*/
function () {
  function ApplicationService() {
    _classCallCheck(this, ApplicationService);
  }

  _createClass(ApplicationService, [{
    key: "getServiceFramework",
    value: function getServiceFramework(controller) {
      var sf = utils["a" /* default */].utilities.sf;
      sf.controller = controller;
      return sf;
    }
  }, {
    key: "getWorkspaces",
    value: function getWorkspaces(callback) {
      var sf = this.getServiceFramework("PBIEmbedded");
      sf.get("GetWorkspaces", {}, callback);
    }
  }, {
    key: "updateWorkspace",
    value: function updateWorkspace(payload, callback, failureCallback) {
      var sf = this.getServiceFramework("PBIEmbedded");
      sf.post("UpdateWorkspace", payload, callback, failureCallback);
    }
  }, {
    key: "deleteWorkspace",
    value: function deleteWorkspace(payload, callback, failureCallback) {
      var sf = this.getServiceFramework("PBIEmbedded");
      sf.post("DeleteWorkspace", payload, callback, failureCallback);
    }
  }, {
    key: "getPowerBiObjectList",
    value: function getPowerBiObjectList(settingsId, callback) {
      var sf = this.getServiceFramework("PBIEmbedded");
      sf.get("GetPowerBiObjectList", {
        settingsId: settingsId
      }, callback);
    }
  }, {
    key: "updatePermissions",
    value: function updatePermissions(payload, callback, failureCallback) {
      var sf = this.getServiceFramework("PBIEmbedded");
      sf.post("SavePowerBiObjectsPermissions", payload, callback, failureCallback);
    }
  }]);

  return ApplicationService;
}();

var applicationService = new applicationService_ApplicationService();
/* harmony default export */ var services_applicationService = (applicationService);
// CONCATENATED MODULE: ./src/actions/settings.js


var settingsActions = {
  switchTab: function switchTab(index, callback) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].SWITCH_TAB,
        payload: index
      });

      if (callback) {
        callback();
      }
    };
  },
  getWorkspaces: function getWorkspaces(callback) {
    return function (dispatch) {
      services_applicationService.getWorkspaces(function (data) {
        dispatch({
          type: actionTypes["a" /* settings */].RETRIEVED_WORKSPACES,
          data: {
            workspaces: data.workspaces,
            clientModified: false
          }
        });

        if (callback) {
          callback(data);
        }
      });
    };
  },
  cancelWorkspaceClientModified: function cancelWorkspaceClientModified() {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].CANCELLED_WORKSPACE_CLIENT_MODIFIED,
        data: {
          workspaceClientModified: false
        }
      });
    };
  },
  workspaceClientModified: function workspaceClientModified(parameter) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].WORKSPACE_CLIENT_MODIFIED,
        data: {
          workspaceDetail: parameter,
          workspaceClientModified: true
        }
      });
    };
  },
  updateWorkspace: function updateWorkspace(payload, callback, failureCallback) {
    return function () {
      services_applicationService.updateWorkspace(payload, function (data) {
        if (callback) {
          callback(data);
        }
      }, function (data) {
        if (failureCallback) {
          failureCallback(data);
        }
      });
    };
  },
  deleteWorkspace: function deleteWorkspace(payload, callback, failureCallback) {
    return function () {
      services_applicationService.deleteWorkspace(payload, function (data) {
        if (callback) {
          callback(data);
        }
      }, function (data) {
        if (failureCallback) {
          failureCallback(data);
        }
      });
    };
  },
  permissionsClientModified: function permissionsClientModified(parameter) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].PERMISSIONS_CLIENT_MODIFIED,
        data: {
          selectedWorkspace: parameter.selectedWorkspace,
          permissions: parameter.permissions,
          workspaceClientModified: true
        }
      });
    };
  },
  getPowerBiObjectList: function getPowerBiObjectList(settingsId, callback) {
    return function (dispatch) {
      services_applicationService.getPowerBiObjectList(settingsId, function (data) {
        dispatch({
          type: actionTypes["a" /* settings */].RETRIEVED_POWERBI_OBJECT_LIST,
          data: {
            inheritPermissions: data.InheritPermissions,
            powerBiObjects: data.PowerBiObjects,
            permissionsClientModified: false
          }
        });

        if (callback) {
          callback(data);
        }
      });
    };
  },
  selectObject: function selectObject(objectId) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].SELECTED_POWERBI_OBJECT,
        data: {
          selectedObjectId: objectId,
          permissionsClientModified: false
        }
      });
    };
  },
  settingsClientModified: function settingsClientModified(settings) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].INHERIT_PERMISSIONS_CHANGED,
        data: {
          inheritPermissions: settings.inheritPermissions,
          permissionsClientModified: true
        }
      });
    };
  },
  permissionsChanged: function permissionsChanged(powerBiObjects) {
    return function (dispatch) {
      dispatch({
        type: actionTypes["a" /* settings */].PERMISSIONS_CHANGED,
        data: {
          powerBiObjects: powerBiObjects,
          permissionsClientModified: true
        }
      });
    };
  },
  updatePermissions: function updatePermissions(payload, callback, failureCallback) {
    return function () {
      services_applicationService.updatePermissions(payload, function (data) {
        if (callback) {
          callback(data);
        }
      }, function (data) {
        if (failureCallback) {
          failureCallback(data);
        }
      });
    };
  }
};
/* harmony default export */ var actions_settings = (settingsActions);
// EXTERNAL MODULE: ./src/components/general/workspaces/workspaceRow/style.less
var style = __webpack_require__(23);

// CONCATENATED MODULE: ./src/components/general/workspaces/workspaceRow/index.jsx
function _typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function workspaceRow_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function workspaceRow_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function workspaceRow_createClass(Constructor, protoProps, staticProps) { if (protoProps) workspaceRow_defineProperties(Constructor.prototype, protoProps); if (staticProps) workspaceRow_defineProperties(Constructor, staticProps); return Constructor; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }






var workspaceRow_WorkspaceRow =
/*#__PURE__*/
function (_Component) {
  _inherits(WorkspaceRow, _Component);

  function WorkspaceRow() {
    workspaceRow_classCallCheck(this, WorkspaceRow);

    return _possibleConstructorReturn(this, _getPrototypeOf(WorkspaceRow).apply(this, arguments));
  }

  workspaceRow_createClass(WorkspaceRow, [{
    key: "componentDidMount",

    /* eslint-disable react/no-did-mount-set-state */
    value: function componentDidMount() {
      var opened = this.props.openId !== "" && this.props.id === this.props.openId;
      this.setState({
        opened: opened
      });
    }
  }, {
    key: "toggle",
    value: function toggle() {
      if (this.props.openId !== "" && this.props.id === this.props.openId) {
        this.props.Collapse();
      } else {
        this.props.OpenCollapse(this.props.id);
      }
    }
    /* eslint-disable react/no-danger */

  }, {
    key: "render",
    value: function render() {
      var props = this.props;
      var opened = this.props.openId !== "" && this.props.id === this.props.openId;
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "collapsible-component-profile" + (opened ? " row-opened" : "")
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "collapsible-profile " + !opened
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "row"
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        title: props.settingsGroupName,
        className: "profile-item item-row-settingsGroupName"
      }, props.settingsGroupName), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "profile-item item-row-authenticationType"
      }, props.authenticationType), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "profile-item item-row-authenticationType"
      }, props.contentPageUrl), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "profile-item item-row-actionButtons"
      }, props.deletable && external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: opened ? "delete-icon-hidden" : "delete-icon",
        dangerouslySetInnerHTML: {
          __html: external_window_dnn_nodeModules_CommonComponents_["SvgIcons"].TrashIcon
        },
        onClick: props.onDelete.bind(this)
      }), props.editable && external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: opened ? "edit-icon-active" : "edit-icon",
        dangerouslySetInnerHTML: {
          __html: external_window_dnn_nodeModules_CommonComponents_["SvgIcons"].EditIcon
        },
        onClick: this.toggle.bind(this)
      })))), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Collapsible"], {
        fixedHeight: 515,
        keepContent: true,
        isOpened: opened,
        style: {
          "float": "left",
          width: "100%",
          overflow: "inherit"
        }
      }, opened && props.children));
    }
  }]);

  return WorkspaceRow;
}(external_window_dnn_nodeModules_React_["Component"]);

workspaceRow_WorkspaceRow.propTypes = {
  settingsId: prop_types_default.a.string,
  settingsGroupId: prop_types_default.a.string,
  settingsGroupName: prop_types_default.a.string,
  authenticationType: prop_types_default.a.string,
  contentPageUrl: prop_types_default.a.string,
  deletable: prop_types_default.a.bool,
  editable: prop_types_default.a.bool,
  OpenCollapse: prop_types_default.a.func,
  Collapse: prop_types_default.a.func,
  onDelete: prop_types_default.a.func,
  id: prop_types_default.a.string,
  openId: prop_types_default.a.string
};
workspaceRow_WorkspaceRow.defaultProps = {
  collapsed: true,
  deletable: true,
  editable: true
};
/* harmony default export */ var workspaceRow = (workspaceRow_WorkspaceRow);
// EXTERNAL MODULE: ./src/components/general/workspaces/workspaceEditor/style.less
var workspaceEditor_style = __webpack_require__(26);

// CONCATENATED MODULE: ./src/resources/index.jsx

var resx = {
  get: function get(key) {
    var moduleName = utils["a" /* default */].moduleName;
    return utils["a" /* default */].utilities.getResx(moduleName, key);
  }
};
/* harmony default export */ var resources = (resx);
// CONCATENATED MODULE: ./src/components/general/workspaces/workspaceEditor/index.jsx
function workspaceEditor_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { workspaceEditor_typeof = function _typeof(obj) { return typeof obj; }; } else { workspaceEditor_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return workspaceEditor_typeof(obj); }

function _extends() { _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; }; return _extends.apply(this, arguments); }

function workspaceEditor_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function workspaceEditor_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function workspaceEditor_createClass(Constructor, protoProps, staticProps) { if (protoProps) workspaceEditor_defineProperties(Constructor.prototype, protoProps); if (staticProps) workspaceEditor_defineProperties(Constructor, staticProps); return Constructor; }

function workspaceEditor_possibleConstructorReturn(self, call) { if (call && (workspaceEditor_typeof(call) === "object" || typeof call === "function")) { return call; } return workspaceEditor_assertThisInitialized(self); }

function workspaceEditor_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function workspaceEditor_getPrototypeOf(o) { workspaceEditor_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return workspaceEditor_getPrototypeOf(o); }

function workspaceEditor_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) workspaceEditor_setPrototypeOf(subClass, superClass); }

function workspaceEditor_setPrototypeOf(o, p) { workspaceEditor_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return workspaceEditor_setPrototypeOf(o, p); }










var workspaceEditor_WorkspaceEditor =
/*#__PURE__*/
function (_Component) {
  workspaceEditor_inherits(WorkspaceEditor, _Component);

  function WorkspaceEditor() {
    var _this;

    workspaceEditor_classCallCheck(this, WorkspaceEditor);

    _this = workspaceEditor_possibleConstructorReturn(this, workspaceEditor_getPrototypeOf(WorkspaceEditor).call(this));
    _this.state = {
      workspaceDetail: {
        SettingsId: 0,
        SettingsGroupId: "",
        SettingsGroupName: "",
        AuthenticationType: "MasterUser",
        Username: "",
        Password: "",
        ApplicationId: "",
        WorkspaceId: "",
        ServicePrincipalApplicationId: "",
        ServicePrincipalApplicationSecret: "",
        ServicePrincipalTenant: "",
        ContentPageUrl: "",
        InheritPermissions: false
      },
      error: {
        SettingsGroupName: false,
        AuthenticationType: false,
        Username: false,
        Password: false,
        ApplicationId: false,
        WorkspaceId: false,
        ServicePrincipalApplicationId: false,
        ServicePrincipalApplicationSecret: false,
        ServicePrincipalTenant: false,
        ContentPageUrl: false
      },
      triedToSubmit: false
    };
    return _this;
  }

  workspaceEditor_createClass(WorkspaceEditor, [{
    key: "componentWillMount",
    value: function componentWillMount() {
      var props = this.props,
          state = this.state;
      state.workspaceDetail["SettingsId"] = props.settingsId || 0;
      state.workspaceDetail["SettingsGroupId"] = props.settingsGroupId || "";
      state.workspaceDetail["SettingsGroupName"] = props.settingsGroupName || "";
      state.workspaceDetail["AuthenticationType"] = props.authenticationType || "MasterUser";
      state.workspaceDetail["Username"] = props.username || "";
      state.workspaceDetail["Password"] = props.password || "";
      state.workspaceDetail["ApplicationId"] = props.applicationId || "";
      state.workspaceDetail["WorkspaceId"] = props.workspaceId || "";
      state.workspaceDetail["ServicePrincipalApplicationId"] = props.servicePrincipalApplicationId || "";
      state.workspaceDetail["ServicePrincipalApplicationSecret"] = props.servicePrincipalApplicationSecret || "";
      state.workspaceDetail["ServicePrincipalTenant"] = props.servicePrincipalTenant || "";
      state.workspaceDetail["ContentPageUrl"] = props.contentPageUrl || "";
      state.workspaceDetail["InheritPermissions"] = props.inheritPermissions;
      state.error["SettingsGroupName"] = props.settingsGroupName === null;
      state.error["AuthenticationType"] = props.authenticationType === null;
      state.error["Username"] = props.username === null;
      state.error["Password"] = props.password === null;
      state.error["ApplicationId"] = props.applicationId === null;
      state.error["WorkspaceId"] = props.workspaceId === null;
      state.error["ServicePrincipalApplicationId"] = props.servicePrincipalApplicationId === null;
      state.error["ServicePrincipalApplicationSecret"] = props.servicePrincipalApplicationSecret === null;
      state.error["ServicePrincipalTenant"] = props.servicePrincipalTenant === null;
      state.error["ContentPageUrl"] = props.contentPageUrl === null;
    }
    /* eslint-disable react/no-did-update-set-state */

  }, {
    key: "componentDidUpdate",
    value: function componentDidUpdate(prevProps) {
      var props = this.props,
          state = this.state;

      if (props !== prevProps && props.workspaceDetail) {
        state.error["SettingsGroupName"] = !props.workspaceDetail["SettingsGroupName"] || props.workspaceDetail["SettingsGroupName"] === "";
        state.error["AuthenticationType"] = !props.workspaceDetail["AuthenticationType"] || props.workspaceDetail["AuthenticationType"] === "";
        state.error["Username"] = props.workspaceDetail["AuthenticationType"] === "MasterUser" && (!props.workspaceDetail["Username"] || props.workspaceDetail["Username"] === "");
        state.error["Password"] = props.workspaceDetail["AuthenticationType"] === "MasterUser" && (!props.workspaceDetail["Password"] || props.workspaceDetail["Password"] === "");
        state.error["ApplicationId"] = !props.workspaceDetail["ApplicationId"] || props.workspaceDetail["ApplicationId"] === "";
        state.error["WorkspaceId"] = !props.workspaceDetail["WorkspaceId"] || props.workspaceDetail["WorkspaceId"] === "";
        state.error["ServicePrincipalApplicationId"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalApplicationId"] || props.workspaceDetail["ServicePrincipalApplicationId"] === "");
        state.error["ServicePrincipalApplicationSecret"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalApplicationSecret"] || props.workspaceDetail["ServicePrincipalApplicationSecret"] === "");
        state.error["ServicePrincipalTenant"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalTenant"] || props.workspaceDetail["ServicePrincipalTenant"] === "");
        state.error["ContentPageUrl"] = !props.workspaceDetail["ContentPageUrl"] || props.workspaceDetail["ContentPageUrl"] === "";
        this.setState({
          workspaceDetail: _extends({}, props.workspaceDetail),
          triedToSubmit: false,
          error: state.error
        });
      }
    }
  }, {
    key: "onSettingChange",
    value: function onSettingChange(key, event) {
      var state = this.state,
          props = this.props;

      var workspaceDetail = _extends({}, state.workspaceDetail);

      switch (key) {
        case "SettingsGroupName":
          state.error[key] = !props.onValidate(workspaceDetail, event);
          break;

        case "Username":
        case "Password":
          state.error[key] = workspaceDetail["AuthenticationType"] === "MasterUser" && event.target.value === "";
          break;

        case "ServicePrincipalApplicationId":
        case "ServicePrincipalApplicationSecret":
        case "ServicePrincipalTenant":
          state.error[key] = workspaceDetail["AuthenticationType"] === "ServicePrincipal" && event.target.value === "";
          break;

        case "AuthenticationType":
          break;

        case "AppicationId":
        case "WorkspaceId":
        case "ContentPageUrl":
        default:
          state.error[key] = event.target.value === "";
          break;
      }

      switch (key) {
        case "SettingsGroupName":
        case "Username":
        case "Password":
        case "AppicationId":
        case "WorkspaceId":
        case "ServicePrincipalApplicationId":
        case "ServicePrincipalApplicationSecret":
        case "ServicePrincipalTenant":
        case "ContentPageUrl":
          workspaceDetail[key] = event.target.value;
          break;

        case "AuthenticationType":
          workspaceDetail[key] = event.value;
          break;

        default:
          workspaceDetail[key] = workspaceEditor_typeof(event) === "object" ? event.target.value : event;
          break;
      }

      this.setState({
        workspaceDetail: workspaceDetail,
        triedToSubmit: false,
        error: state.error
      });
      props.dispatch(actions_settings.workspaceClientModified(workspaceDetail));
    }
  }, {
    key: "getAuthenticationTypeOptions",
    value: function getAuthenticationTypeOptions() {
      var options = [{
        label: "Master User",
        value: "MasterUser"
      }, {
        label: "Service Principal",
        value: "ServicePrincipal"
      }];
      return options;
    }
  }, {
    key: "onSave",
    value: function onSave() {
      var props = this.props,
          state = this.state;
      this.setState({
        triedToSubmit: true
      });

      if (state.error.SettingsGroupName || state.error.AuthenticationType || state.error.AuthenticationType === "MasterUser" && (state.error.Username || state.error.Password) || state.error.ApplicationId || state.error.WorkspaceId || state.error.AuthenticationType === "ServicePrincipal" && (state.error.ServicePrincipalApplicationId || state.error.ServicePrincipalApplicationSecret || state.error.ServicePrincipalTenant) || state.error.ContentPageUrl) {
        return;
      }

      props.onUpdate(state.workspaceDetail);
      props.Collapse();
    }
  }, {
    key: "onCancel",
    value: function onCancel() {
      var props = this.props;

      if (props.workspaceClientModified) {
        utils["a" /* default */].utilities.confirm(resources.get("SettingsRestoreWarning"), resources.get("Yes"), resources.get("No"), function () {
          props.dispatch(actions_settings.cancelWorkspaceClientModified());
          props.Collapse();
        });
      } else {
        props.Collapse();
      }
    }
  }, {
    key: "renderMasterUserCredentials",
    value: function renderMasterUserCredentials() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
        withLabel: true,
        label: resources.get("lblUsername"),
        enabled: true,
        error: this.state.error.Username,
        errorMessage: resources.get("lblUsername.Error"),
        tooltipMessage: resources.get("lblUsername.Help"),
        value: this.state.workspaceDetail.Username,
        onChange: this.onSettingChange.bind(this, "Username")
      }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
        withLabel: true,
        label: resources.get("lblPassword"),
        type: "password",
        enabled: true,
        error: this.state.error.Password,
        errorMessage: resources.get("lblPassword.Error"),
        tooltipMessage: resources.get("lblPassword.Help"),
        value: this.state.workspaceDetail.Password,
        autoComplete: "off",
        onChange: this.onSettingChange.bind(this, "Password")
      }));
    }
  }, {
    key: "renderServicePrincipalCredentials",
    value: function renderServicePrincipalCredentials() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
        withLabel: true,
        label: resources.get("lblServicePrincipalApplicationId"),
        enabled: true,
        error: this.state.error.ServicePrincipalApplicationId,
        errorMessage: resources.get("lblServicePrincipalApplicationId.Error"),
        tooltipMessage: resources.get("lblServicePrincipalApplicationId.Help"),
        value: this.state.workspaceDetail.ServicePrincipalApplicationId,
        onChange: this.onSettingChange.bind(this, "ServicePrincipalApplicationId")
      }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
        withLabel: true,
        label: resources.get("lblServicePrincipalApplicationSecret"),
        type: "password",
        enabled: true,
        error: this.state.error.ServicePrincipalApplicationSecret,
        errorMessage: resources.get("lblServicePrincipalApplicationSecret.Error"),
        tooltipMessage: resources.get("lblServicePrincipalApplicationSecret.Help"),
        value: this.state.workspaceDetail.ServicePrincipalApplicationSecret,
        autoComplete: "off",
        onChange: this.onSettingChange.bind(this, "ServicePrincipalApplicationSecret")
      }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
        withLabel: true,
        label: resources.get("lblServicePrincipalTenant"),
        enabled: true,
        error: this.state.error.ServicePrincipalTenant,
        errorMessage: resources.get("lblServicePrincipalTenant.Error"),
        tooltipMessage: resources.get("lblServicePrincipalTenant.Help"),
        value: this.state.workspaceDetail.ServicePrincipalTenant,
        onChange: this.onSettingChange.bind(this, "ServicePrincipalTenant")
      }));
    }
    /* eslint-disable react/no-danger */

  }, {
    key: "render",
    value: function render() {
      var isMasterUser = this.state.workspaceDetail.AuthenticationType === "MasterUser";

      if (this.state.workspaceDetail !== undefined || this.props.id === "add") {
        var columnOne = external_window_dnn_nodeModules_React_default.a.createElement("div", {
          key: "column-one",
          className: "left-column"
        }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["InputGroup"], null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
          withLabel: true,
          label: resources.get("lblSettingsGroupName"),
          enabled: true,
          error: this.state.error.SettingsGroupName,
          errorMessage: resources.get("lblSettingsGroupName.Error"),
          tooltipMessage: resources.get("lblSettingsGroupName.Help"),
          value: this.state.workspaceDetail.SettingsGroupName,
          onChange: this.onSettingChange.bind(this, "SettingsGroupName")
        }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["DropdownWithError"], {
          withLabel: true,
          label: resources.get("lblAuthenticationType"),
          tooltipMessage: resources.get("lblAuthenticationType.Help"),
          error: this.state.error.AuthenticationType,
          errorMessage: resources.get("ErrorAuthenticationTypeNotValid"),
          options: this.getAuthenticationTypeOptions(),
          value: this.state.workspaceDetail.AuthenticationType,
          onSelect: this.onSettingChange.bind(this, "AuthenticationType")
        }), isMasterUser ? this.renderMasterUserCredentials() : this.renderServicePrincipalCredentials()));
        var columnTwo = external_window_dnn_nodeModules_React_default.a.createElement("div", {
          key: "column-two",
          className: "right-column"
        }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["InputGroup"], null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
          withLabel: true,
          label: resources.get("lblApplicationId"),
          tooltipMessage: resources.get("lblApplicationId.Help"),
          inputStyle: {
            margin: "0"
          },
          error: this.state.error.ApplicationId,
          errorMessage: resources.get("lblApplicationId.Error"),
          value: this.state.workspaceDetail.ApplicationId,
          onChange: this.onSettingChange.bind(this, "ApplicationId")
        }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
          withLabel: true,
          label: resources.get("lblWorkspaceId"),
          tooltipMessage: resources.get("lblWorkspaceId.Help"),
          inputStyle: {
            margin: "0"
          },
          error: this.state.error.WorkspaceId,
          errorMessage: resources.get("lblWorkspaceId.Error"),
          value: this.state.workspaceDetail.WorkspaceId,
          onChange: this.onSettingChange.bind(this, "WorkspaceId")
        }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["SingleLineInputWithError"], {
          withLabel: true,
          label: resources.get("lblContentPageUrl"),
          tooltipMessage: resources.get("lblContentPageUrl.Help"),
          inputStyle: {
            margin: "0"
          },
          error: this.state.error.ContentPageUrl,
          errorMessage: resources.get("lblContentPageUrl.Error"),
          value: this.state.workspaceDetail.ContentPageUrl,
          onChange: this.onSettingChange.bind(this, "ContentPageUrl")
        })));
        return external_window_dnn_nodeModules_React_default.a.createElement("div", {
          className: "workspace-editor"
        }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridSystem"], {
          numberOfColumns: 2
        }, [columnOne, columnTwo]), external_window_dnn_nodeModules_React_default.a.createElement("div", {
          className: "editor-buttons-box"
        }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Button"], {
          type: "secondary",
          onClick: this.onCancel.bind(this)
        }, resources.get("Cancel")), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Button"], {
          type: "primary",
          onClick: this.onSave.bind(this)
        }, resources.get("SaveSettings"))));
      } else return external_window_dnn_nodeModules_React_default.a.createElement("div", null);
    }
  }]);

  return WorkspaceEditor;
}(external_window_dnn_nodeModules_React_["Component"]);

workspaceEditor_WorkspaceEditor.propTypes = {
  dispatch: prop_types_default.a.func.isRequired,
  workspaceDetail: prop_types_default.a.object,
  settingsGroupName: prop_types_default.a.string,
  authenticationType: prop_types_default.a.string,
  username: prop_types_default.a.string,
  password: prop_types_default.a.string,
  applicationId: prop_types_default.a.string,
  workspaceId: prop_types_default.a.string,
  servicePrincipalApplicationId: prop_types_default.a.string,
  servicePrincipalApplicationSecret: prop_types_default.a.string,
  servicePrincipalTenant: prop_types_default.a.string,
  contentPageUrl: prop_types_default.a.string,
  inheritPermissions: prop_types_default.a.bool,
  Collapse: prop_types_default.a.func,
  onUpdate: prop_types_default.a.func,
  id: prop_types_default.a.string,
  workspaceClientModified: prop_types_default.a.bool,
  onValidate: prop_types_default.a.func
};

function mapStateToProps() {
  return {// profileMappingDetail: state.siteBehavior.aliasDetail,
    // profileMappingClientModified: state.siteBehavior.siteAliasClientModified
  };
}

/* harmony default export */ var workspaceEditor = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(mapStateToProps)(workspaceEditor_WorkspaceEditor));
// EXTERNAL MODULE: ./src/components/general/workspaces/style.less
var workspaces_style = __webpack_require__(28);

// CONCATENATED MODULE: ./src/components/general/workspaces/index.jsx
function workspaces_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { workspaces_typeof = function _typeof(obj) { return typeof obj; }; } else { workspaces_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return workspaces_typeof(obj); }

function workspaces_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function workspaces_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function workspaces_createClass(Constructor, protoProps, staticProps) { if (protoProps) workspaces_defineProperties(Constructor.prototype, protoProps); if (staticProps) workspaces_defineProperties(Constructor, staticProps); return Constructor; }

function workspaces_possibleConstructorReturn(self, call) { if (call && (workspaces_typeof(call) === "object" || typeof call === "function")) { return call; } return workspaces_assertThisInitialized(self); }

function workspaces_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function workspaces_getPrototypeOf(o) { workspaces_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return workspaces_getPrototypeOf(o); }

function workspaces_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) workspaces_setPrototypeOf(subClass, superClass); }

function workspaces_setPrototypeOf(o, p) { workspaces_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return workspaces_setPrototypeOf(o, p); }













var workspaces_Workspaces =
/*#__PURE__*/
function (_Component) {
  workspaces_inherits(Workspaces, _Component);

  function Workspaces() {
    var _this;

    workspaces_classCallCheck(this, Workspaces);

    _this = workspaces_possibleConstructorReturn(this, workspaces_getPrototypeOf(Workspaces).call(this));
    _this.state = {
      openId: "",
      tableFields: [],
      error: {
        workspaceSettings: false
      }
    };
    return _this;
  }

  workspaces_createClass(Workspaces, [{
    key: "UNSAFE_componentWillMount",
    value: function UNSAFE_componentWillMount() {
      var props = this.props;
      props.dispatch(actions_settings.getWorkspaces());
    }
  }, {
    key: "UNSAFE_componentWillReceiveProps",
    value: function UNSAFE_componentWillReceiveProps(nextProps) {
      var state = this.state;
      state.error["workspaceSettings"] = nextProps.workspaceSettings === null;
    }
  }, {
    key: "onValidateWorkspaceSetting",
    value: function onValidateWorkspaceSetting(workspaceSettingsDetail, event) {
      if (event.target.value === "") return false;

      if (workspaceSettingsDetail.SettingsGroupName !== event.target.value) {
        // The PropertyName of this row has changed. Let's see if that property has already been mapped
        if (this.props.workspaces.find(function (p) {
          return p.SettingsId !== workspaceSettingsDetail.SettingsId && p.SettingsGroupName === event.target.value;
        }) !== undefined) {
          return false; // Not valid; it's already in the list
        } else {
          return true;
        }
      } else {
        return true;
      }
    }
  }, {
    key: "onUpdateWorkspaceSetting",
    value: function onUpdateWorkspaceSetting(workspaceSettingsDetail) {
      var _this2 = this;

      var props = this.props;
      var payload = {
        workspaceSettingsDetail: workspaceSettingsDetail
      };
      props.dispatch(actions_settings.updateWorkspace(payload, function () {
        utils["a" /* default */].utilities.notify(resources.get("WorkspaceUpdateSuccess"));

        _this2.collapse();

        props.dispatch(actions_settings.getWorkspaces());
      }, function (error) {
        var errorMessage = JSON.parse(error.responseText);
        utils["a" /* default */].utilities.notifyError(errorMessage.Message);
      }));
    }
  }, {
    key: "onDeleteWorkspaceSetting",
    value: function onDeleteWorkspaceSetting(settingsId) {
      var _this3 = this;

      var props = this.props;
      utils["a" /* default */].utilities.confirm(resources.get("WorkspaceSettingDeletedWarning"), resources.get("Yes"), resources.get("No"), function () {
        var payload = {
          SettingsId: settingsId
        };
        props.dispatch(actions_settings.deleteWorkspace(payload, function () {
          utils["a" /* default */].utilities.notify(resources.get("WorkspaceDeleteSuccess"));

          _this3.collapse();

          props.dispatch(actions_settings.getWorkspaces());
        }, function (error) {
          var errorMessage = JSON.parse(error.responseText);
          utils["a" /* default */].utilities.notifyError(errorMessage.Message);
        }));
      });
    }
  }, {
    key: "onClickCancel",
    value: function onClickCancel() {
      utils["a" /* default */].utilities.closePersonaBar();
    }
    /* eslint-disable react/no-did-update-set-state */

  }, {
    key: "componentDidUpdate",
    value: function componentDidUpdate(prevProps) {
      var props = this.props;

      if (props !== prevProps) {
        var tableFields = [];

        if (tableFields.length === 0) {
          tableFields.push({
            "name": resources.get("Workspaces.Id.Header"),
            "id": "SettingsId"
          });
          tableFields.push({
            "name": resources.get("Workspaces.GroupId.Header"),
            "id": "SettingsGroupId"
          });
          tableFields.push({
            "name": resources.get("Workspaces.GroupName.Header"),
            "id": "SettingsGroupName"
          });
          tableFields.push({
            "name": resources.get("Workspaces.AuthenticationType.Header"),
            "id": "AuthenticationType"
          });
          tableFields.push({
            "name": resources.get("Workspaces.ContentPage.Header"),
            "id": "ContentPageUrl"
          });
        }

        this.setState({
          tableFields: tableFields
        });
      }
    }
  }, {
    key: "uncollapse",
    value: function uncollapse(id) {
      this.setState({
        openId: id
      });
    }
  }, {
    key: "collapse",
    value: function collapse() {
      if (this.state.openId !== "") {
        this.setState({
          openId: ""
        });
      }
    }
  }, {
    key: "toggle",
    value: function toggle(openId) {
      if (openId !== "") {
        this.uncollapse(openId);
      }
    }
  }, {
    key: "renderHeader",
    value: function renderHeader() {
      var tableHeaders = this.state.tableFields.map(function (field) {
        var className = "header-" + field.id;
        return external_window_dnn_nodeModules_React_default.a.createElement("div", {
          className: className,
          key: "header-" + field.id
        }, external_window_dnn_nodeModules_React_default.a.createElement("span", null, field.name, "\xA0 "));
      });
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "header-row"
      }, tableHeaders);
    }
  }, {
    key: "renderedWorkspaces",
    value: function renderedWorkspaces() {
      var _this4 = this;

      var i = 0;

      if (this.props.workspaces) {
        return this.props.workspaces.map(function (item, index) {
          var id = "row-" + i++;
          var settingsId = item.SettingsId;
          return external_window_dnn_nodeModules_React_default.a.createElement(workspaceRow, {
            settingsId: settingsId,
            settingsGroupId: item.SettingsGroupId,
            settingsGroupName: item.SettingsGroupName,
            authenticationType: item.AuthenticationType,
            contentPageUrl: item.ContentPageUrl,
            index: index,
            key: "settings-" + index,
            closeOnClick: true,
            openId: _this4.state.openId,
            OpenCollapse: _this4.toggle.bind(_this4),
            Collapse: _this4.collapse.bind(_this4),
            onDelete: _this4.onDeleteWorkspaceSetting.bind(_this4, settingsId),
            id: id
          }, external_window_dnn_nodeModules_React_default.a.createElement(workspaceEditor, {
            workspaceDetail: item,
            settingsId: settingsId,
            settingsGroupId: item.SettingsGroupId,
            settingsGroupName: item.SettingsGroupName,
            authenticationType: item.AuthenticationType,
            username: item.Username,
            password: item.Password,
            servicePrincipalApplicationId: item.ServicePrincipalApplicationId,
            servicePrincipalApplicationSecret: item.ServicePrincipalApplicationSecret,
            servicePrincipalTenant: item.ServicePrincipalTenant,
            applicationId: item.ApplicationId,
            workspaceId: item.WorkspaceId,
            contentPageUrl: item.ContentPageUrl,
            inheritPermissions: item.InheritPermissions,
            Collapse: _this4.collapse.bind(_this4),
            onUpdate: _this4.onUpdateWorkspaceSetting.bind(_this4),
            onValidate: _this4.onValidateWorkspaceSetting.bind(_this4),
            id: id,
            openId: _this4.state.openId
          }));
        });
      }
    }
    /* eslint-disable react/no-danger */

  }, {
    key: "render",
    value: function render() {
      var opened = this.state.openId === "add";
      var newWorkspaceDetail = {
        SettingsId: 0,
        SettingsGroupId: "",
        SettingsGroupName: "",
        AuthenticationType: "MasterUser",
        Username: "",
        Password: "",
        ApplicationId: "",
        WorkspaceId: "",
        ServicePrincipalApplicationId: "",
        ServicePrincipalApplicationSecret: "",
        ServicePrincipalTenant: "",
        ContentPageUrl: "",
        InheritPermissions: false
      };
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dnn-pbiEmbedded-workspaces"
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "workspaces-items"
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "AddItemRow"
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "sectionTitle"
      }, resources.get("lblWorkspaces")), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: opened ? "AddItemBox-active" : "AddItemBox",
        onClick: this.toggle.bind(this, opened ? "" : "add")
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "add-icon",
        dangerouslySetInnerHTML: {
          __html: external_window_dnn_nodeModules_CommonComponents_["SvgIcons"].AddIcon
        }
      }), " ", resources.get("cmdAddWorkspace"))), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "workspaces-items-grid"
      }, this.renderHeader(), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Collapsible"], {
        isOpened: opened,
        style: {
          width: "100%",
          overflow: opened ? "visible" : "hidden"
        }
      }, external_window_dnn_nodeModules_React_default.a.createElement(workspaceRow, {
        settingsGroupName: "-",
        authenticationType: "-",
        contentPageUrl: "-",
        deletable: false,
        editable: false,
        index: "add",
        key: "profileItem-add",
        closeOnClick: true,
        openId: this.state.openId,
        OpenCollapse: this.toggle.bind(this),
        Collapse: this.collapse.bind(this),
        onDelete: this.onDeleteWorkspaceSetting.bind(this),
        id: "add"
      }, external_window_dnn_nodeModules_React_default.a.createElement(workspaceEditor, {
        workspaceDetail: newWorkspaceDetail,
        Collapse: this.collapse.bind(this),
        authenticationType: "MasterUser",
        onUpdate: this.onUpdateWorkspaceSetting.bind(this),
        onValidate: this.onValidateWorkspaceSetting.bind(this),
        id: "add",
        openId: this.state.openId
      }))), this.renderedWorkspaces())));
    }
  }]);

  return Workspaces;
}(external_window_dnn_nodeModules_React_["Component"]);

workspaces_Workspaces.propTypes = {
  workspaces: prop_types_default.a.array
};

function workspaces_mapStateToProps(state) {
  return {
    workspaces: state.settings.workspaces
  };
}

/* harmony default export */ var workspaces = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(workspaces_mapStateToProps)(workspaces_Workspaces));
// EXTERNAL MODULE: ./src/components/general/generalSettings.less
var generalSettings = __webpack_require__(30);

// CONCATENATED MODULE: ./src/components/general/generalSettings.jsx
function generalSettings_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { generalSettings_typeof = function _typeof(obj) { return typeof obj; }; } else { generalSettings_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return generalSettings_typeof(obj); }

function generalSettings_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function generalSettings_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function generalSettings_createClass(Constructor, protoProps, staticProps) { if (protoProps) generalSettings_defineProperties(Constructor.prototype, protoProps); if (staticProps) generalSettings_defineProperties(Constructor, staticProps); return Constructor; }

function generalSettings_possibleConstructorReturn(self, call) { if (call && (generalSettings_typeof(call) === "object" || typeof call === "function")) { return call; } return generalSettings_assertThisInitialized(self); }

function generalSettings_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function generalSettings_getPrototypeOf(o) { generalSettings_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return generalSettings_getPrototypeOf(o); }

function generalSettings_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) generalSettings_setPrototypeOf(subClass, superClass); }

function generalSettings_setPrototypeOf(o, p) { generalSettings_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return generalSettings_setPrototypeOf(o, p); }









var generalSettings_GeneralSettings =
/*#__PURE__*/
function (_Component) {
  generalSettings_inherits(GeneralSettings, _Component);

  function GeneralSettings() {
    var _this;

    generalSettings_classCallCheck(this, GeneralSettings);

    _this = generalSettings_possibleConstructorReturn(this, generalSettings_getPrototypeOf(GeneralSettings).call(this));
    _this.state = {};
    return _this;
  }
  /*UNSAFE_componentWillMount() {
      const {props} = this;        
  }*/

  /*UNSAFE_componentWillReceiveProps(nextProps) {
      const {state} = this;
  } */


  generalSettings_createClass(GeneralSettings, [{
    key: "render",
    value: function render() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dnn-pbiembedded-generalSettings"
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 50
      }, external_window_dnn_nodeModules_React_default.a.createElement("p", {
        className: "panel-description"
      }, resources.get("lblTabDescription"))), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 50
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "logo"
      })), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridSystem"], {
        className: "with-right-border top-half"
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 90
      }, external_window_dnn_nodeModules_React_default.a.createElement("h1", null, resources.get("lblWorkspaceSettings")), external_window_dnn_nodeModules_React_default.a.createElement(workspaces, null)), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 100
      })));
    }
  }]);

  return GeneralSettings;
}(external_window_dnn_nodeModules_React_["Component"]);

generalSettings_GeneralSettings.propTypes = {
  dispatch: prop_types_default.a.func.isRequired
};

function generalSettings_mapStateToProps() {
  return {};
}

/* harmony default export */ var general_generalSettings = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(generalSettings_mapStateToProps)(generalSettings_GeneralSettings));
// CONCATENATED MODULE: ./src/components/general/index.jsx

/* harmony default export */ var general = (general_generalSettings);
// EXTERNAL MODULE: ./src/components/permissions/permissions.less
var permissions_permissions = __webpack_require__(34);

// EXTERNAL MODULE: ./src/components/permissions/pbiObjectsListView/style.less
var pbiObjectsListView_style = __webpack_require__(36);

// CONCATENATED MODULE: ./src/components/permissions/pbiObjectsListView/index.jsx
function pbiObjectsListView_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { pbiObjectsListView_typeof = function _typeof(obj) { return typeof obj; }; } else { pbiObjectsListView_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return pbiObjectsListView_typeof(obj); }

function pbiObjectsListView_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function pbiObjectsListView_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function pbiObjectsListView_createClass(Constructor, protoProps, staticProps) { if (protoProps) pbiObjectsListView_defineProperties(Constructor.prototype, protoProps); if (staticProps) pbiObjectsListView_defineProperties(Constructor, staticProps); return Constructor; }

function pbiObjectsListView_possibleConstructorReturn(self, call) { if (call && (pbiObjectsListView_typeof(call) === "object" || typeof call === "function")) { return call; } return pbiObjectsListView_assertThisInitialized(self); }

function pbiObjectsListView_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function pbiObjectsListView_getPrototypeOf(o) { pbiObjectsListView_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return pbiObjectsListView_getPrototypeOf(o); }

function pbiObjectsListView_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) pbiObjectsListView_setPrototypeOf(subClass, superClass); }

function pbiObjectsListView_setPrototypeOf(o, p) { pbiObjectsListView_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return pbiObjectsListView_setPrototypeOf(o, p); }










var pbiObjectsListView_PbiObjectsListView =
/*#__PURE__*/
function (_Component) {
  pbiObjectsListView_inherits(PbiObjectsListView, _Component);

  function PbiObjectsListView() {
    var _this;

    pbiObjectsListView_classCallCheck(this, PbiObjectsListView);

    _this = pbiObjectsListView_possibleConstructorReturn(this, pbiObjectsListView_getPrototypeOf(PbiObjectsListView).call(this));
    _this.state = {
      selectedWorkspace: "",
      powerBiObjects: null,
      selectedObjectId: "",
      inheritPermissions: true,
      error: {
        selectedWorkspace: false
      }
    };
    return _this;
  }

  pbiObjectsListView_createClass(PbiObjectsListView, [{
    key: "UNSAFE_componentWillMount",
    value: function UNSAFE_componentWillMount() {
      var props = this.props,
          state = this.state;
      state.selectedWorkspace = props.selectedWorkspace || -1;
      state.powerBiObjects = props.powerBiObjects;
      state.workspaces = props.workspaces;
      state.selectedObjectId = props.selectedObjectId || "";
      state.inheritPermissions = props.inheritPermissions;
      props.dispatch(actions_settings.getWorkspaces());
    }
  }, {
    key: "UNSAFE_componentWillReceiveProps",
    value: function UNSAFE_componentWillReceiveProps(nextProps) {
      var state = this.state;
      state.selectedWorkspace = nextProps.selectedWorkspace;
      state.powerBiObjects = nextProps.powerBiObjects;
      state.selectedObjectId = nextProps.selectedObjectId;
      state.inheritPermissions = nextProps.inheritPermissions;
    }
  }, {
    key: "selectObject",
    value: function selectObject(objId) {
      var props = this.props,
          state = this.state;
      state.selectedObjectId = objId;
      props.dispatch(actions_settings.selectObject(objId));
    }
  }, {
    key: "onSettingChange",
    value: function onSettingChange(key) {
      var props = this.props;
      props.dispatch(actions_settings.settingsClientModified({
        inheritPermissions: key === "InheritPermissions" ? !props.inheritPermissions : props.inheritPermissions
      }));
    }
  }, {
    key: "getSelectedObject",
    value: function getSelectedObject(pbiObjects) {
      var state = this.state;
      if (!pbiObjects) pbiObjects = state.powerBiObjects;

      if (state.selectedObjectId !== "" && pbiObjects && pbiObjects.length > 0) {
        return pbiObjects.find(function (x) {
          return x.Id === state.selectedObjectId;
        });
      }

      return null;
    }
  }, {
    key: "getObjects",
    value: function getObjects(objType, noObjectsResKey) {
      var _this2 = this;

      var state = this.state;
      var objects = [];
      if (!state.powerBiObjects) return external_window_dnn_nodeModules_React_default.a.createElement("li", {
        className: "none"
      }, resources.get(noObjectsResKey));
      var pbiObjects = state.powerBiObjects.filter(function (obj) {
        return obj.PowerBiType === objType;
      });

      if (pbiObjects.length > 0) {
        pbiObjects.forEach(function (obj) {
          objects.push(external_window_dnn_nodeModules_React_default.a.createElement("li", null, external_window_dnn_nodeModules_React_default.a.createElement("a", {
            className: "pbiObject" + (obj.Id === state.selectedObjectId ? " selected" : ""),
            onClick: _this2.selectObject.bind(_this2, obj.Id)
          }, obj.Name)));
        });
        return objects;
      } else {
        return external_window_dnn_nodeModules_React_default.a.createElement("li", {
          className: "none"
        }, resources.get(noObjectsResKey));
      }
    }
  }, {
    key: "renderDashboards",
    value: function renderDashboards() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dashboards"
      }, external_window_dnn_nodeModules_React_default.a.createElement("label", null, resources.get("lblDashboards")), external_window_dnn_nodeModules_React_default.a.createElement("ul", null, this.getObjects(1, "NoDashboards")));
    }
  }, {
    key: "renderWorkspace",
    value: function renderWorkspace() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "workspace"
      }, external_window_dnn_nodeModules_React_default.a.createElement("label", null, resources.get("DefaultWorkspacePermissions")), external_window_dnn_nodeModules_React_default.a.createElement("ul", null, this.getObjects(-1, "NoWorkspaces"), external_window_dnn_nodeModules_React_default.a.createElement("li", null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Switch"], {
        label: resources.get("lblInheritPermissions"),
        onText: "",
        offText: "",
        value: this.props.inheritPermissions,
        tooltipMessage: resources.get("lblInheritPermissions.Help"),
        onChange: this.onSettingChange.bind(this, "InheritPermissions")
      }))));
    }
  }, {
    key: "renderReports",
    value: function renderReports() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "reports"
      }, external_window_dnn_nodeModules_React_default.a.createElement("label", null, resources.get("lblReports")), external_window_dnn_nodeModules_React_default.a.createElement("ul", null, this.getObjects(0, "NoReports")));
    }
  }, {
    key: "onPermissionsChanged",
    value: function onPermissionsChanged(permissions) {
      var props = this.props,
          state = this.state;
      var pbiObjects = JSON.parse(JSON.stringify(state.powerBiObjects));
      var selectedObject = this.getSelectedObject(pbiObjects);
      selectedObject.Permissions.rolePermissions = permissions.rolePermissions;
      selectedObject.Permissions.userPermissions = permissions.userPermissions;
      props.dispatch(actions_settings.permissionsChanged(pbiObjects));
    }
  }, {
    key: "getPermissions",
    value: function getPermissions() {
      var permissions = {
        permissionDefinitions: [{
          allowAccess: false,
          fullControl: false,
          permissionCode: null,
          permissionId: 1,
          permissionKey: null,
          permissionName: "View",
          view: true
        }],
        rolePermissions: [],
        userPermissions: []
      };
      var selectedObject = this.getSelectedObject();

      if (selectedObject) {
        permissions = JSON.parse(JSON.stringify(selectedObject.Permissions));
      }

      return permissions;
    }
    /* eslint-disable react/no-danger */

  }, {
    key: "render",
    value: function render() {
      var props = this.props;
      var sf = utils["a" /* default */].utilities.sf;
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dnn-pbiembedded-objectlist"
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "listview"
      }, this.renderWorkspace(), !props.inheritPermissions && this.renderDashboards(), !props.inheritPermissions && this.renderReports()), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "permissions"
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["PermissionGrid"], {
        permissions: this.getPermissions(),
        service: sf,
        onPermissionsChanged: this.onPermissionsChanged.bind(this)
      })));
    }
  }]);

  return PbiObjectsListView;
}(external_window_dnn_nodeModules_React_["Component"]);

pbiObjectsListView_PbiObjectsListView.propTypes = {
  dispatch: prop_types_default.a.func.isRequired,
  powerBiObjects: prop_types_default.a.Object,
  selectedWorkspace: prop_types_default.a.Number,
  selectedObjectId: prop_types_default.a.string,
  inheritPermissions: prop_types_default.a.Boolean
};

function pbiObjectsListView_mapStateToProps(state) {
  return {
    powerBiObjects: state.settings.powerBiObjects,
    selectedWorkspace: state.settings.selectedWorkspace,
    selectedObjectId: state.settings.selectedObjectId,
    inheritPermissions: state.settings.inheritPermissions
  };
}

/* harmony default export */ var pbiObjectsListView = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(pbiObjectsListView_mapStateToProps)(pbiObjectsListView_PbiObjectsListView));
// CONCATENATED MODULE: ./src/components/permissions/permissions.jsx
function permissions_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { permissions_typeof = function _typeof(obj) { return typeof obj; }; } else { permissions_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return permissions_typeof(obj); }

function permissions_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function permissions_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function permissions_createClass(Constructor, protoProps, staticProps) { if (protoProps) permissions_defineProperties(Constructor.prototype, protoProps); if (staticProps) permissions_defineProperties(Constructor, staticProps); return Constructor; }

function permissions_possibleConstructorReturn(self, call) { if (call && (permissions_typeof(call) === "object" || typeof call === "function")) { return call; } return permissions_assertThisInitialized(self); }

function permissions_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function permissions_getPrototypeOf(o) { permissions_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return permissions_getPrototypeOf(o); }

function permissions_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) permissions_setPrototypeOf(subClass, superClass); }

function permissions_setPrototypeOf(o, p) { permissions_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return permissions_setPrototypeOf(o, p); }











var permissions_Permissions =
/*#__PURE__*/
function (_Component) {
  permissions_inherits(Permissions, _Component);

  function Permissions() {
    var _this;

    permissions_classCallCheck(this, Permissions);

    _this = permissions_possibleConstructorReturn(this, permissions_getPrototypeOf(Permissions).call(this));
    _this.state = {
      selectedWorkspace: "",
      selectedObjectId: "",
      powerBiObjects: null,
      workspaces: null,
      inheritPermissions: false,
      error: {
        selectedWorkspace: false
      }
    };
    return _this;
  }

  permissions_createClass(Permissions, [{
    key: "UNSAFE_componentWillMount",
    value: function UNSAFE_componentWillMount() {
      var props = this.props,
          state = this.state;
      state.selectedWorkspace = props.selectedWorkspace || "";
      state.selectedObjectId = props.selectedObjectId || "";
      state.powerBiObjects = props.powerBiObjects;
      state.workspaces = props.workspaces;
      state.inheritPermissions = props.inheritPermissions;
      props.dispatch(actions_settings.getWorkspaces());
    }
  }, {
    key: "UNSAFE_componentWillReceiveProps",
    value: function UNSAFE_componentWillReceiveProps(nextProps) {
      var props = this.props,
          state = this.state;

      if (state.selectedWorkspace === "" && nextProps.workspaces.length > 0) {
        state.selectedWorkspace = nextProps.workspaces[0].SettingsId;
        props.dispatch(actions_settings.getPowerBiObjectList(state.selectedWorkspace));
      }

      state.powerBiObjects = nextProps.powerBiObjects;

      if (state.selectedObjectId === "" && nextProps.powerBiObjects && nextProps.powerBiObjects.length > 0) {
        state.selectedObjectId = nextProps.powerBiObjects[0].Id;
        props.dispatch(actions_settings.selectObject(state.selectedObjectId));
      } else {
        if (state.selectedObjectId === "" && nextProps.powerBiObjects && nextProps.powerBiObjects.length > 0) {
          if (!nextProps.powerBiObjects.find(function (x) {
            return x.Id === state.selectedObjectId;
          })) {
            state.selectedObjectId = "";
            props.dispatch(actions_settings.selectObject(state.selectedObjectId));
          }
        }
      }

      state.error["selectedWorkspace"] = !state.selectedWorkspace || state.selectedWorkspace === "";
    }
  }, {
    key: "onSettingChange",
    value: function onSettingChange(key, event) {
      var state = this.state,
          props = this.props;

      switch (key) {
        case "SelectedWorkspace":
          state.error["selectedWorkspace"] = event.value === "";
          state.selectedWorkspace = event.value;
          state.selectedObjectId = "";
          props.dispatch(actions_settings.getPowerBiObjectList(event.value));
          break;

        default:
          break;
      }

      this.setState({
        selectedWorkspace: state.selectedWorkspace,
        selectedObjectId: state.selectedObjectId,
        workspaces: state.workspaces,
        powerBiObjects: state.powerBiObjects,
        inheritPermissions: state.inheritPermissions,
        triedToSubmit: false
      });
    }
  }, {
    key: "onClickCancel",
    value: function onClickCancel() {
      utils["a" /* default */].utilities.closePersonaBar();
    }
  }, {
    key: "onClickSave",
    value: function onClickSave() {
      var _this2 = this;

      event.preventDefault();
      var props = this.props,
          state = this.state;
      props.dispatch(actions_settings.updatePermissions({
        settingsId: state.selectedWorkspace,
        inheritPermissions: props.inheritPermissions,
        powerBiObjects: props.powerBiObjects
      }, function () {
        utils["a" /* default */].utilities.notify(resources.get("PermissionsUpdateSuccess"));

        _this2.setState({
          clientModified: false
        });
      }, function () {
        utils["a" /* default */].utilities.notifyError(resources.get("PermissionsUpdateError"));
      }));
    }
  }, {
    key: "getWorkspaceOptions",
    value: function getWorkspaceOptions() {
      var options = [];

      if (this.props.workspaces !== undefined) {
        options = this.props.workspaces.map(function (item) {
          return {
            label: item.SettingsGroupName,
            value: item.SettingsId
          };
        });
      }

      return options;
    }
    /* eslint-disable react/no-danger */

  }, {
    key: "render",
    value: function render() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dnn-pbiembedded-permissions"
      }, external_window_dnn_nodeModules_React_default.a.createElement("h1", null, resources.get("lblWorkspacePermissions")), external_window_dnn_nodeModules_React_default.a.createElement("p", null, resources.get("lblPermissionsDescription")), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["InputGroup"], null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridSystem"], {
        numberOfColumns: 2
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 90
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["DropdownWithError"], {
        withLabel: true,
        label: resources.get("lblSelectedWorkspace"),
        tooltipMessage: resources.get("lblSelectedWorkspace.Help"),
        error: this.state.error.selectedWorkspace,
        errorMessage: resources.get("ErrorWorskpaceNotValid"),
        options: this.getWorkspaceOptions(),
        value: this.state.selectedWorkspace,
        onSelect: this.onSettingChange.bind(this, "SelectedWorkspace")
      }), external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "dnn-label"
      }, external_window_dnn_nodeModules_React_default.a.createElement("label", null, resources.get("lblWorkspacePermissions")))), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 100
      }))), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["InputGroup"], null, external_window_dnn_nodeModules_React_default.a.createElement(pbiObjectsListView, {
        selectedObjectId: true
      })), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["InputGroup"], null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["GridCell"], {
        columnSize: 100
      }, external_window_dnn_nodeModules_React_default.a.createElement("div", {
        className: "buttons-box"
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Button"], {
        disabled: false,
        type: "secondary",
        onClick: this.onClickCancel.bind(this)
      }, resources.get("Cancel")), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["Button"], {
        disabled: this.state.error.aadAppClientId || this.state.error.aadAppSecret,
        type: "primary",
        onClick: this.onClickSave.bind(this)
      }, resources.get("SavePermissions"))))));
    }
  }]);

  return Permissions;
}(external_window_dnn_nodeModules_React_["Component"]);

permissions_Permissions.propTypes = {
  dispatch: prop_types_default.a.func.isRequired,
  workspaces: prop_types_default.a.Array,
  powerBiObjects: prop_types_default.a.Object,
  selectedWorkspace: prop_types_default.a.String,
  selectedObjectId: prop_types_default.a.String,
  inheritPermissions: prop_types_default.a.Boolean
};

function permissions_mapStateToProps(state) {
  return {
    workspaces: state.settings.workspaces,
    powerBiObjects: state.settings.powerBiObjects,
    selectedWorkspace: state.settings.selectedWorkspace,
    selectedObjectId: state.settings.selectedObjectId,
    inheritPermissions: state.settings.inheritPermissions
  };
}

/* harmony default export */ var components_permissions_permissions = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(permissions_mapStateToProps)(permissions_Permissions));
// CONCATENATED MODULE: ./src/components/permissions/index.jsx

/* harmony default export */ var components_permissions = (components_permissions_permissions);
// EXTERNAL MODULE: ./src/components/style.less
var components_style = __webpack_require__(38);

// CONCATENATED MODULE: ./src/components/App.jsx
function App_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { App_typeof = function _typeof(obj) { return typeof obj; }; } else { App_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return App_typeof(obj); }

function App_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function App_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function App_createClass(Constructor, protoProps, staticProps) { if (protoProps) App_defineProperties(Constructor.prototype, protoProps); if (staticProps) App_defineProperties(Constructor, staticProps); return Constructor; }

function App_possibleConstructorReturn(self, call) { if (call && (App_typeof(call) === "object" || typeof call === "function")) { return call; } return App_assertThisInitialized(self); }

function App_getPrototypeOf(o) { App_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return App_getPrototypeOf(o); }

function App_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function App_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) App_setPrototypeOf(subClass, superClass); }

function App_setPrototypeOf(o, p) { App_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return App_setPrototypeOf(o, p); }











var App_App =
/*#__PURE__*/
function (_Component) {
  App_inherits(App, _Component);

  function App() {
    var _this;

    App_classCallCheck(this, App);

    _this = App_possibleConstructorReturn(this, App_getPrototypeOf(App).call(this));
    _this.onSelectTab = _this.onSelectTab.bind(App_assertThisInitialized(_this));
    _this.onSelectSubTab = _this.onSelectSubTab.bind(App_assertThisInitialized(_this));
    return _this;
  }

  App_createClass(App, [{
    key: "onSelectTab",
    value: function onSelectTab(index) {
      this.props.dispatch(actions_settings.switchTab(index));
    }
  }, {
    key: "onSelectSubTab",
    value: function onSelectSubTab(index) {
      this.props.dispatch(actions_settings.switchMappingSubTab(index));
    }
  }, {
    key: "render",
    value: function render() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", {
        id: "PBIEmbeddedAppContainer"
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["PersonaBarPage"], {
        isOpen: true
      }, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["PersonaBarPageHeader"], {
        title: "PowerBI Embedded",
        titleCharLimit: 30
      }), external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["PersonaBarPageBody"], null, external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_CommonComponents_["DnnTabs"], {
        onSelect: this.onSelectTab.bind(this),
        selectedIndex: this.props.selectedTab,
        tabHeaders: [resources.get("GeneralSettings"), resources.get("Permissions")]
      }, external_window_dnn_nodeModules_React_default.a.createElement(general, null), external_window_dnn_nodeModules_React_default.a.createElement(components_permissions, null)))));
    }
  }]);

  return App;
}(external_window_dnn_nodeModules_React_["Component"]);

App_App.propTypes = {
  dispatch: prop_types_default.a.func.isRequired,
  selectedTab: prop_types_default.a.number
};

function App_mapStateToProps(state) {
  return {
    selectedTab: state.settings.selectedTab
  };
}

/* harmony default export */ var components_App = (Object(external_window_dnn_nodeModules_ReactRedux_["connect"])(App_mapStateToProps)(App_App));
// EXTERNAL MODULE: ./src/containers/DevTools.js
var DevTools = __webpack_require__(9);

// CONCATENATED MODULE: ./src/containers/Root.dev.js
function Root_dev_typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { Root_dev_typeof = function _typeof(obj) { return typeof obj; }; } else { Root_dev_typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return Root_dev_typeof(obj); }

function Root_dev_classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function Root_dev_defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function Root_dev_createClass(Constructor, protoProps, staticProps) { if (protoProps) Root_dev_defineProperties(Constructor.prototype, protoProps); if (staticProps) Root_dev_defineProperties(Constructor, staticProps); return Constructor; }

function Root_dev_possibleConstructorReturn(self, call) { if (call && (Root_dev_typeof(call) === "object" || typeof call === "function")) { return call; } return Root_dev_assertThisInitialized(self); }

function Root_dev_assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function Root_dev_getPrototypeOf(o) { Root_dev_getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return Root_dev_getPrototypeOf(o); }

function Root_dev_inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) Root_dev_setPrototypeOf(subClass, superClass); }

function Root_dev_setPrototypeOf(o, p) { Root_dev_setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return Root_dev_setPrototypeOf(o, p); }





var Root_dev_Root =
/*#__PURE__*/
function (_Component) {
  Root_dev_inherits(Root, _Component);

  function Root() {
    Root_dev_classCallCheck(this, Root);

    return Root_dev_possibleConstructorReturn(this, Root_dev_getPrototypeOf(Root).apply(this, arguments));
  }

  Root_dev_createClass(Root, [{
    key: "render",
    value: function render() {
      return external_window_dnn_nodeModules_React_default.a.createElement("div", null, external_window_dnn_nodeModules_React_default.a.createElement(components_App, null), external_window_dnn_nodeModules_React_default.a.createElement(DevTools["a" /* default */], null));
    }
  }]);

  return Root;
}(external_window_dnn_nodeModules_React_["Component"]);

/* harmony default export */ var Root_dev = __webpack_exports__["default"] = (Root_dev_Root);

/***/ }),
/* 41 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
// ESM COMPAT FLAG
__webpack_require__.r(__webpack_exports__);

// EXTERNAL MODULE: external "window.dnn.nodeModules.React"
var external_window_dnn_nodeModules_React_ = __webpack_require__(0);
var external_window_dnn_nodeModules_React_default = /*#__PURE__*/__webpack_require__.n(external_window_dnn_nodeModules_React_);

// EXTERNAL MODULE: external "window.dnn.nodeModules.ReactDOM"
var external_window_dnn_nodeModules_ReactDOM_ = __webpack_require__(12);

// EXTERNAL MODULE: external "window.dnn.nodeModules.ReactRedux"
var external_window_dnn_nodeModules_ReactRedux_ = __webpack_require__(5);

// EXTERNAL MODULE: ./src/utils/index.jsx
var utils = __webpack_require__(3);

// CONCATENATED MODULE: ./src/globals/application.js

var boilerPlate = {
  init: function init() {
    // This setting is required and define the public path 
    // to allow the web application to download assets on demand 
    // eslint-disable-next-line no-undef
    // __webpack_public_path__ = options.publicPath;        
    var options = window.dnn.initPbiEmbedded();
    utils["a" /* default */].init(options.utility);
    utils["a" /* default */].moduleName = options.moduleName;
  },
  dispatch: function dispatch() {
    throw new Error("dispatch method needs to be overwritten from the Redux store");
  }
};
/* harmony default export */ var application = (boilerPlate);
// EXTERNAL MODULE: external "window.dnn.nodeModules.Redux"
var external_window_dnn_nodeModules_Redux_ = __webpack_require__(8);

// EXTERNAL MODULE: external "window.dnn.nodeModules.ReduxThunk"
var external_window_dnn_nodeModules_ReduxThunk_ = __webpack_require__(13);
var external_window_dnn_nodeModules_ReduxThunk_default = /*#__PURE__*/__webpack_require__.n(external_window_dnn_nodeModules_ReduxThunk_);

// EXTERNAL MODULE: external "window.dnn.nodeModules.ReduxImmutableStateInvariant"
var external_window_dnn_nodeModules_ReduxImmutableStateInvariant_ = __webpack_require__(14);
var external_window_dnn_nodeModules_ReduxImmutableStateInvariant_default = /*#__PURE__*/__webpack_require__.n(external_window_dnn_nodeModules_ReduxImmutableStateInvariant_);

// EXTERNAL MODULE: ./src/constants/actionTypes/index.js + 1 modules
var actionTypes = __webpack_require__(4);

// CONCATENATED MODULE: ./src/reducers/settingsReducer.js
function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; var ownKeys = Object.keys(source); if (typeof Object.getOwnPropertySymbols === 'function') { ownKeys = ownKeys.concat(Object.getOwnPropertySymbols(source).filter(function (sym) { return Object.getOwnPropertyDescriptor(source, sym).enumerable; })); } ownKeys.forEach(function (key) { _defineProperty(target, key, source[key]); }); } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }


function settings() {
  var state = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {
    selectedTab: 0,
    workspaces: null,
    powerBiObjects: null,
    selectedWorkspace: ""
  };
  var action = arguments.length > 1 ? arguments[1] : undefined;

  switch (action.type) {
    case actionTypes["a" /* settings */].RETRIEVED_WORKSPACES:
      return _objectSpread({}, state, {
        workspaces: action.data.workspaces,
        clientModified: action.data.clientModified
      });

    case actionTypes["a" /* settings */].UPDATED_WORKSPACE:
      return _objectSpread({}, state, {
        clientModified: action.data.clientModified
      });

    case actionTypes["a" /* settings */].SWITCH_TAB:
      return _objectSpread({}, state, {
        selectedTab: action.payload
      });

    case actionTypes["a" /* settings */].CANCELLED_WORKSPACE_CLIENT_MODIFIED:
      return _objectSpread({}, state, {
        workspaceClientModified: action.data.workspaceClientModified
      });

    case actionTypes["a" /* settings */].WORKSPACE_CLIENT_MODIFIED:
      return _objectSpread({}, state, {
        workspaceDetail: action.data.workspaceDetail,
        workspaceClientModified: action.data.workspaceClientModified
      });

    case actionTypes["a" /* settings */].RETRIEVED_POWERBI_OBJECT_LIST:
      return _objectSpread({}, state, {
        inheritPermissions: action.data.inheritPermissions,
        powerBiObjects: action.data.powerBiObjects,
        permissionsClientModified: action.data.permissionsClientModified
      });

    case actionTypes["a" /* settings */].SELECTED_POWERBI_OBJECT:
      return _objectSpread({}, state, {
        selectedObjectId: action.data.selectedObjectId,
        permissionsClientModified: action.data.permissionsClientModified
      });

    case actionTypes["a" /* settings */].INHERIT_PERMISSIONS_CHANGED:
      return _objectSpread({}, state, {
        inheritPermissions: action.data.inheritPermissions,
        permissionsClientModified: action.data.permissionsClientModified
      });

    case actionTypes["a" /* settings */].PERMISSIONS_CHANGED:
      return _objectSpread({}, state, {
        powerBiObjects: action.data.powerBiObjects,
        permissionsClientModified: action.data.permissionsClientModified
      });

    default:
      return _objectSpread({}, state);
  }
}
// CONCATENATED MODULE: ./src/reducers/rootReducer.js


var rootReducer = Object(external_window_dnn_nodeModules_Redux_["combineReducers"])({
  settings: settings
});
/* harmony default export */ var reducers_rootReducer = (rootReducer);
// EXTERNAL MODULE: ./src/containers/DevTools.js
var DevTools = __webpack_require__(9);

// CONCATENATED MODULE: ./src/store/configureStore.js





function configureStore(initialState) {
  var store = Object(external_window_dnn_nodeModules_Redux_["createStore"])(reducers_rootReducer, initialState, Object(external_window_dnn_nodeModules_Redux_["compose"])(Object(external_window_dnn_nodeModules_Redux_["applyMiddleware"])(external_window_dnn_nodeModules_ReduxThunk_default.a, external_window_dnn_nodeModules_ReduxImmutableStateInvariant_default()()), // TODO: apply only for development          
  DevTools["a" /* default */].instrument()));
  return store;
}
// EXTERNAL MODULE: ./src/containers/Root.js
var Root = __webpack_require__(18);
var Root_default = /*#__PURE__*/__webpack_require__.n(Root);

// CONCATENATED MODULE: ./src/main.jsx






var main_store = configureStore({
  enabled: false,
  instrumentationKey: ""
});
application.dispatch = main_store.dispatch;
var appContainer = document.getElementById("pbiEmbedded-container");
var initCallback = appContainer.getAttribute("data-init-callback");
application.init(initCallback);
Object(external_window_dnn_nodeModules_ReactDOM_["render"])(external_window_dnn_nodeModules_React_default.a.createElement(external_window_dnn_nodeModules_ReactRedux_["Provider"], {
  store: main_store
}, external_window_dnn_nodeModules_React_default.a.createElement(Root_default.a, null)), appContainer);

/***/ })
/******/ ]);
//# sourceMappingURL=bundle-en.js.map
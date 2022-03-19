/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ([
/* 0 */,
/* 1 */
/***/ ((module) => {

module.exports = require("vscode");

/***/ })
/******/ 	]);
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
var exports = __webpack_exports__;

Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.activate = void 0;
const vscode = __webpack_require__(1);
function activate(context) {
    context.subscriptions.push(vscode.languages.registerDocumentSymbolProvider({ language: "GCB" }, new GCBDocumentSymbolProvider()));
}
exports.activate = activate;
class GCBDocumentSymbolProvider {
    provideDocumentSymbols(document, token) {
        return new Promise((resolve, reject) => {
            var symbols = [];
            var remblock = false;
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#chip\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*#chip", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: "Part " + symname[1],
                            kind: vscode.SymbolKind.Class,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#config\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*#config", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Property,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#define\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*#define", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Constant,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdim\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*dim", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Variable,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdir\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*dir", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Event,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("\\b\\s*(\\S+)(?::)$", "i");
                var remregex = new RegExp("(?:[';]|rem).*:", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Namespace,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bsub\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*sub", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Method,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bmacro\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*macro", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Method,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bfunction\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*function", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Function,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\btable\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*table", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Interface,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#script\\s+)(\\S+)", "i");
                var remregex = new RegExp("(?:[';]|rem).*script", "i");
                var startremblockregex = new RegExp("[/][*]", "i");
                var endremblockregex = new RegExp("[*][/]", "i");
                if (startremblockregex.test(line.text)) {
                    remblock = true;
                }
                if (endremblockregex.test(line.text)) {
                    remblock = false;
                }
                if (!remregex.test(line.text) && !remblock) {
                    if (regex.test(line.text)) {
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname[1],
                            kind: vscode.SymbolKind.Key,
                            location: new vscode.Location(document.uri, line.range)
                        });
                    }
                }
            }
            resolve(symbols);
        });
    }
}

})();

module.exports = __webpack_exports__;
/******/ })()
;
//# sourceMappingURL=extension.js.map
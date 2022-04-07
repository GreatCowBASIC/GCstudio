"use strict";(function(){function n(u,f,l,p){const r=/("[^"\\]*(?:\\.[^"\\]*)*")|('[^'\\]*(?:\\.[^'\\]*)*')|(\/\*[^\/\*]*(?:(?:\*|\/)[^\/\*]*)*?\*\/)|(\/{2,}.*?(?:(?:\r?\n)|$))/g;function o(c){return c.replace(r,function(i,d,m,s,e){if(s)return"";if(e){const t=e.length;return e[t-1]===`
`?e[t-2]==="\r"?`\r
`:`
`:""}else return i})}return{stripComments:o}}typeof define=="function"?define([],function(){return n()}):typeof module=="object"&&typeof module.exports=="object"?module.exports=n():console.trace("strip comments defined in UNKNOWN context (neither requirejs or commonjs)")})();

//# sourceMappingURL=https://ticino.blob.core.windows.net/sourcemaps/e18005f0f1b33c29e81d732535d8c0e47cafb0b5/core/vs\base\common\stripComments.js.map

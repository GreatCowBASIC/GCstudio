'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
function activate(context) {
    var enableExtension = false;
    vscode.commands.registerCommand('extension.enableHoverconverter', () => {
        vscode.window.showInformationMessage('Hover Converter Enabled');
        enableExtension = true;
    });
    vscode.commands.registerCommand('extension.disableHoverconverter', () => {
        enableExtension = false;
        vscode.window.showInformationMessage('Hover Converter Disabled');
    });
    var regexHex = /^0x[0-9a-fA-F]+$/g;
    var regexHexc = /^[0-9a-fA-F]+[h]$/g;
    var regexDec = /^-?[0-9]+$/g;
    let hover = vscode.languages.registerHoverProvider({ scheme: '*', language: '*' }, {
        provideHover(document, position, token) {
            var hoveredWord = document.getText(document.getWordRangeAtPosition(position));
            var markdownString = new vscode.MarkdownString();
            if ((regexHex.test(hoveredWord.toString()) || regexHexc.test(hoveredWord.toString())) && enableExtension) {
                markdownString.appendCodeblock(`Dec:\n${parseInt(hoveredWord, 16)}\nBinary:\n${parseInt(hoveredWord, 16).toString(2)}`, 'javascript');
                return {
                    contents: [markdownString]
                };
            }
            else if (regexDec.test(hoveredWord.toString()) && enableExtension) {
                var input = Number(hoveredWord.toString());
                markdownString.appendCodeblock(`Hex:\n0x${input.toString(16).toUpperCase()}\nBinary:\n${input.toString(2).replace(/(^\s+|\s+$)/, '')} `, 'javascript');
                return {
                    contents: [markdownString]
                };
            }
        }
    });
    context.subscriptions.push(hover);
}
exports.activate = activate;
function deactivate() { }
exports.deactivate = deactivate;
//# sourceMappingURL=extension.js.map
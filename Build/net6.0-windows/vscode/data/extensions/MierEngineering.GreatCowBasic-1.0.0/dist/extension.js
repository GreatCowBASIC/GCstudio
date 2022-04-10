'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
exports.deactivate = exports.activate = void 0;
const vscode = require("vscode");
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
// let fs = require("fs");
const vscode_1 = require("vscode");
const path_1 = require("path");
var init = false;
var hasCpp = false;
const extensionId = "gcb";
// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
function activate(context) {
    //Symbol Provider
    context.subscriptions.push(vscode.languages.registerDocumentSymbolProvider({ language: "GCB" }, new GCBDocumentSymbolProvider()));
    //Menu Bar
    if (!init) {
        init = true;
        vscode_1.commands.getCommands().then(function (value) {
            let result = value.indexOf("C_Cpp.SwitchHeaderSource");
            if (result >= 0) {
                hasCpp = true;
            }
        });
    }
    console.log("extension is now active!");
    // rest of code
    // Step: If simple commands then add to this array
    let commandArray = [
        //=> ["name in package.json" , "name of command to execute"]
        ["MenuBar.save", "workbench.action.files.save"],
        [
            "MenuBar.toggleTerminal",
            "workbench.action.terminal.toggleTerminal",
        ],
        [
            "MenuBar.toggleActivityBar",
            "workbench.action.toggleActivityBarVisibility",
        ],
        ["MenuBar.navigateBack", "workbench.action.navigateBack"],
        ["MenuBar.navigateForward", "workbench.action.navigateForward"],
        [
            "MenuBar.toggleRenderWhitespace",
            "editor.action.toggleRenderWhitespace",
        ],
        ["MenuBar.quickOpen", "workbench.action.quickOpen"],
        ["MenuBar.findReplace", "editor.action.startFindReplaceAction"],
        ["MenuBar.undo", "undo"],
        ["MenuBar.redo", "redo"],
        ["MenuBar.commentLine", "editor.action.commentLine"],
        ["MenuBar.saveAll", "workbench.action.files.saveAll"],
        ["MenuBar.openFile", "workbench.action.files.openFile"],
        ["MenuBar.newFile", "workbench.action.files.newUntitledFile"],
        ["MenuBar.goToDefinition", "editor.action.revealDefinition"],
        ["MenuBar.cut", "editor.action.clipboardCutAction"],
        ["MenuBar.copy", "editor.action.clipboardCopyAction"],
        ["MenuBar.paste", "editor.action.clipboardPasteAction"],
        [
            "MenuBar.compareWithSaved",
            "workbench.files.action.compareWithSaved",
        ],
        ["MenuBar.showCommands", "workbench.action.showCommands"],
        ["MenuBar.startDebugging", "workbench.action.debug.start"],
        ["MenuBar.indentLines", "editor.action.indentLines"],
        ["MenuBar.outdentLines", "editor.action.outdentLines"],
        ["MenuBar.openSettings", "workbench.action.openSettings"],
        ["MenuBar.toggleWordWrap", "editor.action.toggleWordWrap"],
        ["MenuBar.taskMenu", "workbench.action.tasks.runTask"],
    ];
    let disposableCommandsArray = [];
    // The command has been defined in the package.json file
    // Now provide the implementation of the command with  registerCommand
    // The commandId parameter must match the command field in package.json
    commandArray.forEach((command) => {
        disposableCommandsArray.push(vscode_1.commands.registerCommand(command[0], () => {
            vscode_1.commands.executeCommand(command[1]).then(function () { });
        }));
    });
    // Step: else add complex command separately
    // Make Hex and Flash
    let disposablehexflash = vscode_1.commands.registerCommand("MenuBar.hexflash", () => {
        var args = "Make HEX and Flash [F5]";
        vscode_1.commands
            .executeCommand("workbench.action.tasks.runTask", args)
            .then(function () { });
    });
    // Make Hex
    let disposablehex = vscode_1.commands.registerCommand("MenuBar.hex", () => {
        var args = "Make HEX [F6]";
        vscode_1.commands
            .executeCommand("workbench.action.tasks.runTask", args)
            .then(function () { });
    });
    // Make ASM
    let disposableasm = vscode_1.commands.registerCommand("MenuBar.asm", () => {
        var args = "Make ASM [F7]";
        vscode_1.commands
            .executeCommand("workbench.action.tasks.runTask", args)
            .then(function () { });
    });
    // Make Flash
    let disposableflash = vscode_1.commands.registerCommand("MenuBar.flash", () => {
        var args = "Flash Previous";
        vscode_1.commands
            .executeCommand("workbench.action.tasks.runTask", args)
            .then(function () { });
    });
    let disposableBeautify = vscode_1.commands.registerCommand("MenuBar.beautify", () => {
        let editor = vscode_1.window.activeTextEditor;
        if (!editor) {
            return; // No open text editor
        }
        if (vscode_1.window.state.focused === true && !editor.selection.isEmpty) {
            vscode_1.commands
                .executeCommand("editor.action.formatSelection")
                .then(function () { });
        }
        else {
            vscode_1.commands
                .executeCommand("editor.action.formatDocument")
                .then(function () { });
        }
    });
    let disposableFormatWith = vscode_1.commands.registerCommand("MenuBar.formatWith", () => {
        let editor = vscode_1.window.activeTextEditor;
        if (!editor) {
            return; // No open text editor
        }
        if (vscode_1.window.state.focused === true && !editor.selection.isEmpty) {
            vscode_1.commands
                .executeCommand("editor.action.formatSelection.multiple")
                .then(function () { });
        }
        else {
            vscode_1.commands
                .executeCommand("editor.action.formatDocument.multiple")
                .then(function () { });
        }
    });
    let disposableSwitch = vscode_1.commands.registerCommand("MenuBar.switchHeaderSource", () => {
        if (hasCpp) {
            vscode_1.commands
                .executeCommand("C_Cpp.SwitchHeaderSource")
                .then(function () { });
        }
        else {
            vscode_1.window.showErrorMessage("C/C++ extension (ms-vscode.cpptools) is not installed!");
        }
    });
    // Adding 1) to a list of disposables which are disposed when this extension is deactivated
    disposableCommandsArray.forEach((i) => {
        context.subscriptions.push(i);
    });
    // Adding 2) to a list of disposables which are disposed when this extension is deactivated
    // context.subscriptions.push(disposableFileList);
    context.subscriptions.push(disposableBeautify);
    context.subscriptions.push(disposableFormatWith);
    context.subscriptions.push(disposableSwitch);
    // Adding 3 // user defined userButtons
    for (let index = 1; index <= 10; index++) {
        const printIndex = index !== 10 ? "0" + index : "" + index;
        let action = "userButton" + printIndex;
        let actionName = "MenuBar." + action;
        let disposableUserButtonCommand = vscode_1.commands.registerCommand(actionName, () => {
            const config = vscode_1.workspace.getConfiguration("MenuBar");
            let configName = action + "Command";
            const command = config.get(configName);
            // skip userButtons not set
            if (command === null ||
                command === undefined ||
                command.trimEnd() === "") {
                return;
            }
            const palettes = command.split(",");
            executeNext(action, palettes, 0);
        });
        context.subscriptions.push(disposableUserButtonCommand);
    }
    //also update userButton in package.json.. see "Adding new userButtons" in help.md file
    //Binary/Hex/Decimal Converter Code:
    var regexHex = /^0x[0-9a-fA-F]+$/g;
    var regexHexc = /^[0-9a-fA-F]+[h]$/g;
    var regexDec = /^-?[0-9]+$/g;
    var regexBin = /^0b[01]+$/g;
    let hover = vscode.languages.registerHoverProvider({ scheme: '*', language: '*' }, {
        provideHover(document, position, token) {
            var hoveredWord = document.getText(document.getWordRangeAtPosition(position));
            var markdownString = new vscode.MarkdownString();
            if (regexBin.test(hoveredWord.toString())) {
                var input = Number(parseInt(hoveredWord.substring(2), 2).toString());
                markdownString.appendCodeblock(`Dec:\n${input}\nHex:\n0x${input.toString(16).toUpperCase()} `, 'javascript');
                return {
                    contents: [markdownString]
                };
            }
            else if (regexHex.test(hoveredWord.toString()) || regexHexc.test(hoveredWord.toString())) {
                markdownString.appendCodeblock(`Dec:\n${parseInt(hoveredWord, 16)}\nBinary:\n${parseInt(hoveredWord, 16).toString(2)}`, 'javascript');
                return {
                    contents: [markdownString]
                };
            }
            else if (regexDec.test(hoveredWord.toString())) {
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
// this method is called when your extension is deactivated
function deactivate() { }
exports.deactivate = deactivate;
//Symbol provider
class GCBDocumentSymbolProvider {
    provideDocumentSymbols(document, token) {
        return new Promise((resolve, reject) => {
            var symbols = [];
            var remblock = false;
            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#chip\\s+)(\\w+)", "i");
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
// Integrated Menu Bar
// local functions for user-defined button execution follow, based on
// https://github.com/ppatotski/vscode-commandbar/ Copyright 2018 Petr Patotski
function executeNext(action, palettes, index) {
    try {
        let [cmd, ...args] = palettes[index].split("|");
        if (args) {
            args = args.map((arg) => resolveVariables(arg));
        }
        cmd = cmd.trim();
        vscode_1.commands.executeCommand(cmd, ...args).then(() => {
            index++;
            if (index < palettes.length) {
                executeNext(action, palettes, index);
            }
        }, (err) => {
            vscode_1.window.showErrorMessage(`Execution of '${action}' command has failed: ${err.message}`);
        });
    }
    catch (err) {
        vscode_1.window.showErrorMessage(`Execution of '${action}' command has failed: ${err.message}`);
        console.error(err);
    }
}
const resolveVariablesFunctions = {
    env: (name) => process.env[name.toUpperCase()],
    cwd: () => process.cwd(),
    workspaceRoot: () => getWorkspaceFolder(),
    workspaceFolder: () => getWorkspaceFolder(),
    workspaceRootFolderName: () => path_1.basename(getWorkspaceFolder()),
    workspaceFolderBasename: () => path_1.basename(getWorkspaceFolder()),
    lineNumber: () => { var _a; return (_a = vscode_1.window.activeTextEditor) === null || _a === void 0 ? void 0 : _a.selection.active.line; },
    selectedText: () => {
        var _a;
        return (_a = vscode_1.window.activeTextEditor) === null || _a === void 0 ? void 0 : _a.document.getText(vscode_1.window.activeTextEditor.selection);
    },
    file: () => getActiveEditorName(),
    fileDirname: () => path_1.dirname(getActiveEditorName()),
    fileExtname: () => path_1.extname(getActiveEditorName()),
    fileBasename: () => path_1.basename(getActiveEditorName()),
    fileBasenameNoExtension: () => {
        const edtBasename = path_1.basename(getActiveEditorName());
        return edtBasename.slice(0, edtBasename.length - path_1.extname(edtBasename).length);
    },
    execPath: () => process.execPath,
};
const variableRegEx = /\$\{(.*?)\}/g;
function resolveVariables(commandLine) {
    return commandLine
        .trim()
        .replace(variableRegEx, function replaceVariable(match, variableValue) {
        const [variable, argument] = variableValue.split(":");
        const resolver = resolveVariablesFunctions[variable];
        if (!resolver) {
            throw new Error(`Variable ${variable} not found!`);
        }
        return resolver(argument);
    });
}
function getActiveEditorName() {
    if (vscode_1.window.activeTextEditor) {
        return vscode_1.window.activeTextEditor.document.fileName;
    }
    return "";
}
function getWorkspaceFolder(activeTextEditor = vscode_1.window.activeTextEditor) {
    let folder;
    if (vscode_1.workspace === null || vscode_1.workspace === void 0 ? void 0 : vscode_1.workspace.workspaceFolders) {
        if (vscode_1.workspace.workspaceFolders.length === 1) {
            folder = vscode_1.workspace.workspaceFolders[0].uri.fsPath;
        }
        else if (activeTextEditor) {
            const folderObject = vscode_1.workspace.getWorkspaceFolder(activeTextEditor.document.uri);
            if (folderObject) {
                folder = folderObject.uri.fsPath;
            }
            else {
                folder = "";
            }
        }
        else if (vscode_1.workspace.workspaceFolders.length > 0) {
            folder = vscode_1.workspace.workspaceFolders[0].uri.fsPath;
        }
    }
    return folder;
}
//# sourceMappingURL=extension.js.map
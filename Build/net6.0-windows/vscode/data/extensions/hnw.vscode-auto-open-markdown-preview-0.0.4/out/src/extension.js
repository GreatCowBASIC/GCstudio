'use strict';
var vscode = require('vscode');
var vscode_1 = require('vscode');
function activate(context) {
    var alreadyOpenedFirstMarkdown = false;
    var markdown_preview_command_id = "";
    var close_other_editor_command_id = "";
    close_other_editor_command_id = "workbench.action.closeEditorsInOtherGroups";
    markdown_preview_command_id = "markdown.showPreviewToSide";
    function previewFirstMarkdown() {
        if (alreadyOpenedFirstMarkdown) {
            return;
        }
        var editor = vscode_1.window.activeTextEditor;
        if (editor) {
            var doc = editor.document;
            if (doc && doc.languageId === "markdown") {
                openMarkdownPreviewSideBySide();
                alreadyOpenedFirstMarkdown = true;
            }
        }
    }
    function openMarkdownPreviewSideBySide() {
        vscode_1.commands.executeCommand(close_other_editor_command_id)
            .then(function () { return vscode_1.commands.executeCommand(markdown_preview_command_id); })
            .then(function () { }, function (e) { return console.error(e); });
    }
    if (vscode_1.window.activeTextEditor) {
        previewFirstMarkdown();
    }
    else {
        vscode.window.onDidChangeActiveTextEditor(function () {
            previewFirstMarkdown();
        });
    }
    vscode.workspace.onDidOpenTextDocument(function (doc) {
        if (doc && doc.languageId === "markdown") {
            openMarkdownPreviewSideBySide();
        }
    });
}
exports.activate = activate;
function deactivate() {
}
exports.deactivate = deactivate;
//# sourceMappingURL=extension.js.map
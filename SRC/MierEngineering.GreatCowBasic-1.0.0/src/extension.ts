'use strict';
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(vscode.languages.registerDocumentSymbolProvider(
        {language: "GCB"}, new GCBDocumentSymbolProvider()
    ));
}

class GCBDocumentSymbolProvider implements vscode.DocumentSymbolProvider {
    public provideDocumentSymbols(document: vscode.TextDocument,
            token: vscode.CancellationToken): Thenable<vscode.SymbolInformation[]> {



        return new Promise((resolve, reject) => {
            var symbols:any = [];
            var remblock:boolean = false;



            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#chip\\s+)(\\w+)","i");
                var remregex = new RegExp("(?:[';]|rem).*#chip","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: "Part " + symname![1],
                            kind: vscode.SymbolKind.Class,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#config\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*#config","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Property,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#define\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*#define","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Constant,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdim\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*dim","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Variable,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdir\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*dir","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Event,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("\\b\\s*(\\S+)(?::)$","i");
                var remregex = new RegExp("(?:[';]|rem).*:","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {                
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Namespace,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }


            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bsub\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*sub","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Method,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bmacro\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*macro","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Method,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bfunction\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*function","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {                
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Function,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }


            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\btable\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*table","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Interface,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#script\\s+)(\\S+)","i");
                var remregex = new RegExp("(?:[';]|rem).*script","i");
                var startremblockregex = new RegExp("[/][*]","i");
                var endremblockregex = new RegExp("[*][/]","i");
                if (startremblockregex.test(line.text))
                {
                    remblock = true;
                }
                if (endremblockregex.test(line.text))
                {
                    remblock = false;
                }

                if (!remregex.test(line.text) && !remblock)
                {
                    if (regex.test(line.text)) {                  
                        var symname = regex.exec(line.text);
                        symbols.push({
                            name: symname![1],
                            kind: vscode.SymbolKind.Key,
                            location: new vscode.Location(document.uri, line.range)
                        })
                    }
                }
            }
           

            resolve(symbols);
        });
    }
}
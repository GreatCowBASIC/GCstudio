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




            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#chip\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: "Part " + symname![1],
                        kind: vscode.SymbolKind.Class,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#config\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Property,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#define\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Constant,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdim\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Variable,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bdir\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Event,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("\\b\\s*(\\S+)(?::)$","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Namespace,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }


            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bsub\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Method,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bmacro\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Method,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\bfunction\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Function,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }


            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:\\btable\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Interface,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }

            for (var i = 0; i < document.lineCount; i++) {
                var line = document.lineAt(i);
                var regex = new RegExp("(?:#script\\s+)(\\S+)","i");
                
                if (regex.test(line.text)) {                  
                    var symname = regex.exec(line.text);
                    symbols.push({
                        name: symname![1],
                        kind: vscode.SymbolKind.Key,
                        location: new vscode.Location(document.uri, line.range)
                    })
                }
            }
           

            resolve(symbols);
        });
    }
}
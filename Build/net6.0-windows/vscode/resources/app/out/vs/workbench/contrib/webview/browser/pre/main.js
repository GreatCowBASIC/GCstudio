const isSafari=navigator.vendor&&navigator.vendor.indexOf("Apple")>-1&&navigator.userAgent&&navigator.userAgent.indexOf("CriOS")===-1&&navigator.userAgent.indexOf("FxiOS")===-1,isFirefox=navigator.userAgent&&navigator.userAgent.indexOf("Firefox")>=0,searchParams=new URL(location.toString()).searchParams,ID=searchParams.get("id"),onElectron=searchParams.get("platform")==="electron",expectedWorkerVersion=parseInt(searchParams.get("swVersion")),parentOrigin=searchParams.get("parentOrigin"),trackFocus=({onFocus:e,onBlur:o})=>{const n=250;let t=document.hasFocus();setInterval(()=>{const r=document.hasFocus();r!==t&&(t=r,r?e():o())},n)},getActiveFrame=()=>document.getElementById("active-frame"),getPendingFrame=()=>document.getElementById("pending-frame");function assertIsDefined(e){if(typeof e=="undefined"||e===null)throw new Error("Found unexpected null");return e}const vscodePostMessageFuncName="__vscode_post_message__",defaultStyles=document.createElement("style");defaultStyles.id="_defaultStyles",defaultStyles.textContent=`
	html {
		scrollbar-color: var(--vscode-scrollbarSlider-background) var(--vscode-editor-background);
	}

	body {
		background-color: transparent;
		color: var(--vscode-editor-foreground);
		font-family: var(--vscode-font-family);
		font-weight: var(--vscode-font-weight);
		font-size: var(--vscode-font-size);
		margin: 0;
		padding: 0 20px;
	}

	img {
		max-width: 100%;
		max-height: 100%;
	}

	a, a code {
		color: var(--vscode-textLink-foreground);
	}

	a:hover {
		color: var(--vscode-textLink-activeForeground);
	}

	a:focus,
	input:focus,
	select:focus,
	textarea:focus {
		outline: 1px solid -webkit-focus-ring-color;
		outline-offset: -1px;
	}

	code {
		color: var(--vscode-textPreformat-foreground);
	}

	blockquote {
		background: var(--vscode-textBlockQuote-background);
		border-color: var(--vscode-textBlockQuote-border);
	}

	kbd {
		color: var(--vscode-editor-foreground);
		border-radius: 3px;
		vertical-align: middle;
		padding: 1px 3px;

		background-color: hsla(0,0%,50%,.17);
		border: 1px solid rgba(71,71,71,.4);
		border-bottom-color: rgba(88,88,88,.4);
		box-shadow: inset 0 -1px 0 rgba(88,88,88,.4);
	}
	.vscode-light kbd {
		background-color: hsla(0,0%,87%,.5);
		border: 1px solid hsla(0,0%,80%,.7);
		border-bottom-color: hsla(0,0%,73%,.7);
		box-shadow: inset 0 -1px 0 hsla(0,0%,73%,.7);
	}

	::-webkit-scrollbar {
		width: 10px;
		height: 10px;
	}

	::-webkit-scrollbar-corner {
		background-color: var(--vscode-editor-background);
	}

	::-webkit-scrollbar-thumb {
		background-color: var(--vscode-scrollbarSlider-background);
	}
	::-webkit-scrollbar-thumb:hover {
		background-color: var(--vscode-scrollbarSlider-hoverBackground);
	}
	::-webkit-scrollbar-thumb:active {
		background-color: var(--vscode-scrollbarSlider-activeBackground);
	}
	::highlight(find-highlight) {
		background-color: var(--vscode-editor-findMatchHighlightBackground);
	}
	::highlight(current-find-highlight) {
		background-color: var(--vscode-editor-findMatchBackground);
	}`;function getVsCodeApiScript(e,o){const n=o?encodeURIComponent(o):void 0;return`
			globalThis.acquireVsCodeApi = (function() {
				const originalPostMessage = window.parent['${vscodePostMessageFuncName}'].bind(window.parent);
				const doPostMessage = (channel, data, transfer) => {
					originalPostMessage(channel, data, transfer);
				};

				let acquired = false;

				let state = ${o?`JSON.parse(decodeURIComponent("${n}"))`:void 0};

				return () => {
					if (acquired && !${e}) {
						throw new Error('An instance of the VS Code API has already been acquired');
					}
					acquired = true;
					return Object.freeze({
						postMessage: function(message, transfer) {
							doPostMessage('onmessage', { message, transfer }, transfer);
						},
						setState: function(newState) {
							state = newState;
							doPostMessage('do-update-state', JSON.stringify(newState));
							return newState;
						},
						getState: function() {
							return state;
						}
					});
				};
			})();
			delete window.parent;
			delete window.top;
			delete window.frameElement;
		`}const workerReady=new Promise((e,o)=>{if(!areServiceWorkersEnabled())return o(new Error("Service Workers are not enabled. Webviews will not work. Try disabling private/incognito mode."));const n=`service-worker.js?v=${expectedWorkerVersion}&vscode-resource-base-authority=${searchParams.get("vscode-resource-base-authority")}&remoteAuthority=${searchParams.get("remoteAuthority")??""}`;navigator.serviceWorker.register(n).then(()=>navigator.serviceWorker.ready).then(async t=>{const r=async d=>{if(d.data.channel==="version")return navigator.serviceWorker.removeEventListener("message",r),d.data.version===expectedWorkerVersion?e():(console.log(`Found unexpected service worker version. Found: ${d.data.version}. Expected: ${expectedWorkerVersion}`),console.log("Attempting to reload service worker"),t.unregister().then(()=>navigator.serviceWorker.register(n)).then(()=>navigator.serviceWorker.ready).finally(()=>{e()}))};navigator.serviceWorker.addEventListener("message",r);const s=d=>{d.postMessage({channel:"version"})},c=navigator.serviceWorker.controller;if(c?.scriptURL.endsWith(n))s(c);else{const d=()=>{navigator.serviceWorker.removeEventListener("controllerchange",d),s(navigator.serviceWorker.controller)};navigator.serviceWorker.addEventListener("controllerchange",d)}}).catch(t=>{o(new Error(`Could not register service workers: ${t}.`))})}),hostMessaging=new class{constructor(){this.channel=new MessageChannel,this.handlers=new Map,this.channel.port1.onmessage=o=>{const n=o.data.channel,t=this.handlers.get(n);if(t)for(const r of t)r(o,o.data.args);else console.log("no handler for ",o)}}postMessage(o,n,t){this.channel.port1.postMessage({channel:o,data:n},t)}onMessage(o,n){let t=this.handlers.get(o);t||(t=[],this.handlers.set(o,t)),t.push(n)}signalReady(){window.parent.postMessage({target:ID,channel:"webview-ready",data:{}},parentOrigin,[this.channel.port2])}},unloadMonitor=new class{constructor(){this.confirmBeforeClose="keyboardOnly",this.isModifierKeyDown=!1,hostMessaging.onMessage("set-confirm-before-close",(e,o)=>{this.confirmBeforeClose=o}),hostMessaging.onMessage("content",(e,o)=>{this.confirmBeforeClose=o.confirmBeforeClose}),window.addEventListener("beforeunload",e=>{if(!onElectron)switch(this.confirmBeforeClose){case"always":return e.preventDefault(),e.returnValue="","";case"never":break;case"keyboardOnly":default:{if(this.isModifierKeyDown)return e.preventDefault(),e.returnValue="","";break}}})}onIframeLoaded(e){e.contentWindow.addEventListener("keydown",o=>{this.isModifierKeyDown=o.metaKey||o.ctrlKey||o.altKey}),e.contentWindow.addEventListener("keyup",()=>{this.isModifierKeyDown=!1})}};let firstLoad=!0,loadTimeout,styleVersion=0,pendingMessages=[];const initData={initialScrollProgress:void 0,styles:void 0,activeTheme:void 0,themeName:void 0};hostMessaging.onMessage("did-load-resource",(e,o)=>{navigator.serviceWorker.ready.then(n=>{assertIsDefined(n.active).postMessage({channel:"did-load-resource",data:o},o.data?.buffer?[o.data.buffer]:[])})}),hostMessaging.onMessage("did-load-localhost",(e,o)=>{navigator.serviceWorker.ready.then(n=>{assertIsDefined(n.active).postMessage({channel:"did-load-localhost",data:o})})}),navigator.serviceWorker.addEventListener("message",e=>{switch(e.data.channel){case"load-resource":case"load-localhost":hostMessaging.postMessage(e.data.channel,e.data);return}});const applyStyles=(e,o)=>{if(!!e&&(o&&(o.classList.remove("vscode-light","vscode-dark","vscode-high-contrast"),initData.activeTheme&&o.classList.add(initData.activeTheme),o.dataset.vscodeThemeKind=initData.activeTheme,o.dataset.vscodeThemeName=initData.themeName||""),initData.styles)){const n=e.documentElement.style;for(let t=n.length-1;t>=0;t--){const r=n[t];r&&r.startsWith("--vscode-")&&n.removeProperty(r)}for(const t of Object.keys(initData.styles))n.setProperty(`--${t}`,initData.styles[t])}},handleInnerClick=e=>{if(!e?.view?.document)return;const o=e.view.document.querySelector("base");for(const n of e.composedPath()){const t=n;if(t.tagName&&t.tagName.toLowerCase()==="a"&&t.href){if(t.getAttribute("href")==="#")e.view.scrollTo(0,0);else if(t.hash&&(t.getAttribute("href")===t.hash||o&&t.href===o.href+t.hash)){const r=t.hash.slice(1);(e.view.document.getElementById(r)??e.view.document.getElementById(decodeURIComponent(r)))?.scrollIntoView()}else hostMessaging.postMessage("did-click-link",t.href.baseVal||t.href);e.preventDefault();return}}},handleAuxClick=e=>{if(!!e?.view?.document&&e.button===1)for(const o of e.composedPath()){const n=o;if(n.tagName&&n.tagName.toLowerCase()==="a"&&n.href){e.preventDefault();return}}},handleInnerKeydown=e=>{if(isUndoRedo(e)||isPrint(e)||isFindEvent(e))e.preventDefault();else if(isCopyPasteOrCut(e))if(onElectron)e.preventDefault();else return;hostMessaging.postMessage("did-keydown",{key:e.key,keyCode:e.keyCode,code:e.code,shiftKey:e.shiftKey,altKey:e.altKey,ctrlKey:e.ctrlKey,metaKey:e.metaKey,repeat:e.repeat})},handleInnerUp=e=>{hostMessaging.postMessage("did-keyup",{key:e.key,keyCode:e.keyCode,code:e.code,shiftKey:e.shiftKey,altKey:e.altKey,ctrlKey:e.ctrlKey,metaKey:e.metaKey,repeat:e.repeat})};function isCopyPasteOrCut(e){const o=e.ctrlKey||e.metaKey,n=e.shiftKey&&e.key.toLowerCase()==="insert";return o&&["c","v","x"].includes(e.key.toLowerCase())||n}function isUndoRedo(e){return(e.ctrlKey||e.metaKey)&&["z","y"].includes(e.key.toLowerCase())}function isPrint(e){return(e.ctrlKey||e.metaKey)&&e.key.toLowerCase()==="p"}function isFindEvent(e){return(e.ctrlKey||e.metaKey)&&e.key.toLowerCase()==="f"}let isHandlingScroll=!1;const handleWheel=e=>{isHandlingScroll||hostMessaging.postMessage("did-scroll-wheel",{deltaMode:e.deltaMode,deltaX:e.deltaX,deltaY:e.deltaY,deltaZ:e.deltaZ,detail:e.detail,type:e.type})},handleInnerScroll=e=>{if(isHandlingScroll)return;const o=e.target,n=e.currentTarget;if(!n||!o?.body)return;const t=n.scrollY/o.body.clientHeight;isNaN(t)||(isHandlingScroll=!0,window.requestAnimationFrame(()=>{try{hostMessaging.postMessage("did-scroll",t)}catch{}isHandlingScroll=!1}))};function onDomReady(e){document.readyState==="interactive"||document.readyState==="complete"?e():document.addEventListener("DOMContentLoaded",e)}function areServiceWorkersEnabled(){try{return!!navigator.serviceWorker}catch{return!1}}function toContentHtml(e){const o=e.options,n=e.contents,t=new DOMParser().parseFromString(n,"text/html");if(t.querySelectorAll("a").forEach(s=>{if(!s.title){const c=s.getAttribute("href");typeof c=="string"&&(s.title=c)}}),t.body.hasAttribute("role")||t.body.setAttribute("role","document"),o.allowScripts){const s=t.createElement("script");s.id="_vscodeApiScript",s.textContent=getVsCodeApiScript(o.allowMultipleAPIAcquire,e.state),t.head.prepend(s)}t.head.prepend(defaultStyles.cloneNode(!0)),applyStyles(t,t.body);for(const s of Array.from(t.querySelectorAll("meta"))){const c=s.getAttribute("http-equiv");c&&!/^(content-security-policy|default-style|content-type)$/i.test(c)&&(console.warn(`Removing unsupported meta http-equiv: ${c}`),s.remove())}const r=t.querySelector('meta[http-equiv="Content-Security-Policy"]');if(!r)hostMessaging.postMessage("no-csp-found");else try{const s=r.getAttribute("content");if(s){const c=s.replace(/(vscode-webview-resource|vscode-resource):(?=(\s|;|$))/g,e.cspSource);r.setAttribute("content",c)}}catch(s){console.error(`Could not rewrite csp: ${s}`)}return`<!DOCTYPE html>
`+t.documentElement.outerHTML}onDomReady(()=>{if(!document.body)return;hostMessaging.onMessage("styles",(n,t)=>{++styleVersion,initData.styles=t.styles,initData.activeTheme=t.activeTheme,initData.themeName=t.themeName;const r=getActiveFrame();!r||r.contentDocument&&applyStyles(r.contentDocument,r.contentDocument.body)}),hostMessaging.onMessage("focus",()=>{const n=getActiveFrame();if(!n||!n.contentWindow){window.focus();return}document.activeElement!==n&&n.contentWindow.focus()});let e=0;hostMessaging.onMessage("content",async(n,t)=>{const r=++e;try{await workerReady}catch(a){console.error(`Webview fatal error: ${a}`),hostMessaging.postMessage("fatal-error",{message:a+""});return}if(r!==e)return;const s=t.options,c=toContentHtml(t),d=styleVersion,f=getActiveFrame(),k=firstLoad;let h;if(firstLoad)firstLoad=!1,h=(a,i)=>{typeof initData.initialScrollProgress=="number"&&!isNaN(initData.initialScrollProgress)&&i.scrollY===0&&i.scroll(0,a.clientHeight*initData.initialScrollProgress)};else{const a=f&&f.contentDocument&&f.contentDocument.body?assertIsDefined(f.contentWindow).scrollY:0;h=(i,l)=>{l.scrollY===0&&l.scroll(0,a)}}const m=getPendingFrame();m&&(m.setAttribute("id",""),document.body.removeChild(m)),k||(pendingMessages=[]);const u=document.createElement("iframe");u.setAttribute("id","pending-frame"),u.setAttribute("frameborder","0");const g=new Set(["allow-same-origin","allow-pointer-lock"]);s.allowScripts&&(g.add("allow-scripts"),g.add("allow-downloads")),s.allowForms&&g.add("allow-forms"),u.setAttribute("sandbox",Array.from(g).join(" ")),isFirefox||u.setAttribute("allow",s.allowScripts?"clipboard-read; clipboard-write;":""),u.src=`./fake.html?id=${ID}`,u.style.cssText="display: block; margin: 0; overflow: hidden; position: absolute; width: 100%; height: 100%; visibility: hidden",document.body.appendChild(u);function p(a){setTimeout(()=>{a.open(),a.write(c),a.close(),M(u),d!==styleVersion&&applyStyles(a,a.body)},0)}if(!s.allowScripts&&isSafari){const a=setInterval(()=>{if(!u.parentElement){clearInterval(a);return}const i=assertIsDefined(u.contentDocument);i.location.pathname.endsWith("/fake.html")&&i.readyState!=="loading"&&(clearInterval(a),p(i))},10)}else assertIsDefined(u.contentWindow).addEventListener("DOMContentLoaded",a=>{const i=a.target?a.target:void 0;p(assertIsDefined(i))});const y=(a,i)=>{a&&a.body&&h(a.body,i);const l=getPendingFrame();if(l&&l.contentDocument&&l.contentDocument===a){const v=document.hasFocus(),w=getActiveFrame();w&&document.body.removeChild(w),d!==styleVersion&&applyStyles(l.contentDocument,l.contentDocument.body),l.setAttribute("id","active-frame"),l.style.visibility="visible",i.addEventListener("scroll",handleInnerScroll),i.addEventListener("wheel",handleWheel),v&&i.focus(),pendingMessages.forEach(b=>{i.postMessage(b.message,window.origin,b.transfer)}),pendingMessages=[]}};function M(a){clearTimeout(loadTimeout),loadTimeout=void 0,loadTimeout=setTimeout(()=>{clearTimeout(loadTimeout),loadTimeout=void 0,y(assertIsDefined(a.contentDocument),assertIsDefined(a.contentWindow))},200);const i=assertIsDefined(a.contentWindow);i.addEventListener("load",function(l){const v=l.target;loadTimeout&&(clearTimeout(loadTimeout),loadTimeout=void 0,y(v,this))}),i.addEventListener("click",handleInnerClick),i.addEventListener("auxclick",handleAuxClick),i.addEventListener("keydown",handleInnerKeydown),i.addEventListener("keyup",handleInnerUp),i.addEventListener("contextmenu",l=>{l.defaultPrevented||(l.preventDefault(),hostMessaging.postMessage("did-context-menu",{clientX:l.clientX,clientY:l.clientY}))}),unloadMonitor.onIframeLoaded(a)}}),hostMessaging.onMessage("message",(n,t)=>{if(!getPendingFrame()){const s=getActiveFrame();if(s){assertIsDefined(s.contentWindow).postMessage(t.message,window.origin,t.transfer);return}}pendingMessages.push(t)}),hostMessaging.onMessage("initial-scroll-position",(n,t)=>{initData.initialScrollProgress=t}),hostMessaging.onMessage("execCommand",(n,t)=>{const r=getActiveFrame();!r||assertIsDefined(r.contentDocument).execCommand(t)});let o;hostMessaging.onMessage("find",(n,t)=>{const r=getActiveFrame();if(!r)return;if(!t.previous&&o!==t.value){const c=r.contentWindow.getSelection();c.collapse(c.anchorNode)}o=t.value;const s=r.contentWindow.find(t.value,!1,t.previous,!0,!1,!1,!1);hostMessaging.postMessage("did-find",s)}),hostMessaging.onMessage("find-stop",(n,t)=>{const r=getActiveFrame();if(!!r&&(o=void 0,!t.clearSelection)){const s=r.contentWindow.getSelection();for(let c=0;c<s.rangeCount;c++)s.removeRange(s.getRangeAt(c))}}),trackFocus({onFocus:()=>hostMessaging.postMessage("did-focus"),onBlur:()=>hostMessaging.postMessage("did-blur")}),window[vscodePostMessageFuncName]=(n,t)=>{switch(n){case"onmessage":case"do-update-state":hostMessaging.postMessage(n,t);break}},hostMessaging.signalReady()});

//# sourceMappingURL=https://ticino.blob.core.windows.net/sourcemaps/b5205cc8eb4fbaa726835538cd82372cc0222d43/core/vs\workbench\contrib\webview\browser\pre\main.js.map

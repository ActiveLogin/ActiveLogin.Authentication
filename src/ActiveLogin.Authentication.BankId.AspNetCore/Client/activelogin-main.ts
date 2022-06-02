interface IBankIdUiScriptConfiguration {
    initialStatusMessage: string;
    unknownErrorMessage: string;
    unsupportedBrowserErrorMessage: string;

    bankIdInitializeApiUrl: string;
    bankIdStatusApiUrl: string;
    bankIdQrCodeApiUrl: string;
    bankIdCancelApiUrl: string;

    statusRefreshIntervalMs: number;
    qrCodeRefreshIntervalMs: number;
}

interface IBankIdUiScriptInitState {
    antiXsrfRequestToken: string;

    returnUrl: string;
    cancelReturnUrl: string;

    protectedUiOptions: string;
}

function activeloginInit(configuration: IBankIdUiScriptConfiguration, initState: IBankIdUiScriptInitState) {
    // Pre check

    const requiredFeatures = [window.fetch, window.sessionStorage];
    const isMissingSomeFeature = requiredFeatures.some(x => !x);
    if (isMissingSomeFeature) {
        showStatus(configuration.unsupportedBrowserErrorMessage, "danger", false);
        return;
    }

    // QR

    var qrLastRefreshTimestamp : Date = null;
    var qrIsRefreshing = false;

    // OrderRef

    const sessionStorageOrderRefKey = "ActiveLogin_BankId_OrderRef";
    const sessionOrderRef = sessionStorage.getItem(sessionStorageOrderRefKey);
    sessionStorage.removeItem(sessionStorageOrderRefKey);

    // Elements

    const uiWrapperElement = <HTMLElement>document.querySelector(".activelogin-bankid-ui--wrapper");
    const formElement = <HTMLFormElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--form");

    const statusWrapperElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-wrapper");
    const statusInfoElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-info");
    const statusSpinnerElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-spinner");
    const statusMessageElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-message");
    const cancelButtonElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--cancel-button");
    const qrCodeElement = <HTMLImageElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--qr-code-image");

    const uiResultForm = <HTMLFormElement>document.querySelector("form[name=activelogin-bankid-ui--result-form]");
    const uiResultInput = <HTMLInputElement>uiResultForm.querySelector("input[name=uiResult]");

    const startBankIdAppButtonElement = <HTMLButtonElement>uiWrapperElement.querySelector(".activelogin-bankid-ui-startapp-button");

    // Events

    if (sessionOrderRef) {
        showOrderRefStatus(sessionOrderRef);
    } else {
        document.addEventListener("DOMContentLoaded", () => {
            resetUi();
            login();
        });
    }

    // Boot

    function showOrderRefStatus(orderRef : string) {
        showStatus(configuration.initialStatusMessage, "white", true);
        checkStatus(
            initState.antiXsrfRequestToken,
            initState.returnUrl,
            initState.protectedUiOptions,
            orderRef
        );
    }

    function login() {
        initialize(
            initState.antiXsrfRequestToken,
            initState.returnUrl,
            initState.cancelReturnUrl,
            initState.protectedUiOptions
        );
    }

    function resetUi() {
        hide(qrCodeElement);
    }

    // BankID

    var autoStartAttempts = 0;
    var loginIsCancelledByUser = false;

    function initialize(requestVerificationToken: string, returnUrl: string, cancelUrl: string, protectedUiOptions: string) {
        loginIsCancelledByUser = false;

        function enableCancelButton(orderRef : string = null) {
            var onCancelButtonClick = (event : Event) => {
                cancel(requestVerificationToken, cancelUrl, protectedUiOptions, orderRef);
                event.target.removeEventListener("click", onCancelButtonClick);
            };
            cancelButtonElement.addEventListener("click", onCancelButtonClick);
        }

        postJson(configuration.bankIdInitializeApiUrl,
            requestVerificationToken,
            {
                "returnUrl": returnUrl,
                "uiOptions": protectedUiOptions
            })
            .then(data => {
                if (data.isAutoLaunch) {
                    if (!data.checkStatus) {
                        sessionStorage.setItem(sessionStorageOrderRefKey, data.orderRef);
                    }

                    if (data.deviceMightRequireUserInteractionToLaunchBankIdApp) {
                        var startBankIdAppButtonOnClick = (event : Event) => {
                            window.location.href = data.redirectUri;
                            hide(startBankIdAppButtonElement);
                            event.target.removeEventListener("click", startBankIdAppButtonOnClick);
                        };
                        startBankIdAppButtonElement.addEventListener("click", startBankIdAppButtonOnClick);

                        show(startBankIdAppButtonElement);
                    } else {
                        window.location.href = data.redirectUri;
                    }
                }

                if (!!data.qrStartState && !!data.qrCodeAsBase64) {
                    setQrCode(data.qrCodeAsBase64);
                    qrLastRefreshTimestamp = new Date();
                    refreshQrCode(requestVerificationToken, data.qrStartState);
                }

                enableCancelButton(data.orderRef);

                hide(formElement);
                showStatus(configuration.initialStatusMessage, "white", true);

                if (data.checkStatus) {
                    checkStatus(requestVerificationToken, returnUrl, protectedUiOptions, data.orderRef);
                }
            })
            .catch(error => {
                showStatus(error.message, "danger", false);
                hide(qrCodeElement);
                hide(startBankIdAppButtonElement);
                enableCancelButton();
            });
    }

    function checkStatus(requestVerificationToken: string, returnUrl: string, protectedUiOptions: string, orderRef: string) {
        if (loginIsCancelledByUser) {
            return;
        }

        postJson(configuration.bankIdStatusApiUrl,
            requestVerificationToken,
            {
                "orderRef": orderRef,
                "returnUrl": returnUrl,
                "uiOptions": protectedUiOptions,
                "autoStartAttempts": autoStartAttempts
            })
            .then(data => {
                if (data.retryLogin) {
                    autoStartAttempts++;
                    login();
                } else if (data.isFinished) {
                    uiResultForm.setAttribute("action", data.redirectUri);
                    uiResultInput.value = data.result;
                    uiResultForm.submit();
                } else if (!loginIsCancelledByUser) {
                    autoStartAttempts = 0;
                    showStatus(data.statusMessage, "white", true);
                    setTimeout(() => {
                        checkStatus(requestVerificationToken, returnUrl, protectedUiOptions, orderRef);
                    }, configuration.statusRefreshIntervalMs);
                }
            })
            .catch(error => {
                if (!loginIsCancelledByUser) {
                    showStatus(error.message, "danger", false);
                    hide(startBankIdAppButtonElement);
                }
                hide(qrCodeElement);
            });
    }

    function refreshQrCode(requestVerificationToken: string, qrStartState: string) {
        if (loginIsCancelledByUser || qrIsRefreshing) {
            return;
        }
        
        const currentTime = new Date();
        const timeSinceLastRefresh = currentTime.getTime() - qrLastRefreshTimestamp.getTime();
        if (timeSinceLastRefresh < configuration.qrCodeRefreshIntervalMs) {
            setTimeout(() => {
                    refreshQrCode(requestVerificationToken, qrStartState);
            }, configuration.qrCodeRefreshIntervalMs);
            return;
        }
        qrIsRefreshing = true;

        postJson(configuration.bankIdQrCodeApiUrl,
            requestVerificationToken,
            {
                "qrStartState": qrStartState
            })
            .then(data => {
                if (!!data.qrCodeAsBase64) {
                    qrLastRefreshTimestamp = new Date();
                    setQrCode(data.qrCodeAsBase64);
                    setTimeout(() => {
                            refreshQrCode(requestVerificationToken, qrStartState);
                    }, configuration.qrCodeRefreshIntervalMs);
                }
            })
            .catch(error => {
                if (!loginIsCancelledByUser) {
                    showStatus(error.message, "danger", false);
                    hide(startBankIdAppButtonElement);
                }
                hide(qrCodeElement);
            })
            .finally(() => {
                qrIsRefreshing = false;
            });
    }

    function setQrCode(qrCodeAsBase64 : string) {
        qrCodeElement.src = 'data:image/png;base64, ' + qrCodeAsBase64;
        show(qrCodeElement);
    }

    function cancel(requestVerificationToken: string, cancelReturnUrl: string, protectedUiOptions: string, orderRef: string = null) {
        loginIsCancelledByUser = true;

        if (!orderRef) {
            window.location.href = cancelReturnUrl;
            return;
        }

        postJson(configuration.bankIdCancelApiUrl,
            requestVerificationToken,
            {
                "orderRef": orderRef,
                "uiOptions": protectedUiOptions
            })
            .finally(() => {
                window.location.href = cancelReturnUrl;
            });
    }

    // Helpers

    function postJson(url: string, requestVerificationToken: string, data: any) {
        return fetch(url,
            {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "RequestVerificationToken": requestVerificationToken
                },
                credentials: 'include',
                body: JSON.stringify(data)
            })
            .then(response => {
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json();
                }

                throw Error(configuration.unknownErrorMessage);
            })
            .then(data => {
                if (!!data.errorMessage) {
                    throw Error(data.errorMessage);
                }
                return data;
            });
    }

    function showStatus(status: string, type: string, spinner : boolean) {
        let textClass = "text-white";
        if (type === "white") {
            textClass = "";
        }

        statusInfoElement.className = `card activelogin-bankid-ui--status-info bg-${type} ${textClass}`;
        statusMessageElement.innerText = status;
        setVisibility(statusSpinnerElement, spinner, "inline-block");
        show(statusWrapperElement);
    }

    function setVisibility(element : HTMLElement, visible : boolean, display : string = null) {
        if (visible) {
            show(element, display);
        } else {
            hide(element);
        }
    }

    function show(element : HTMLElement, display : string = "block") {
        if (!element) {
            return;
        }

        element.style.display = display;
    }

    function hide(element : HTMLElement) {
        if (!element) {
            return;
        }

        element.style.display = "none";
    }
}

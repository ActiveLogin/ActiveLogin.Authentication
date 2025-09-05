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
    const fetchRetryCountDefault: number = 3;
    const fetchRetryDelayMs: number = 1000;

    // Pre check

    const requiredFeatures = [window.fetch, window.sessionStorage];
    const isMissingSomeFeature = requiredFeatures.some(x => !x);
    if (isMissingSomeFeature) {
        showErrorStatus(configuration.unsupportedBrowserErrorMessage);
        return;
    }

    // QR

    var qrLastRefreshTimestamp: Date = null;
    var qrIsRefreshing = false;
    var qrRefreshTimeoutId: number = null;

    // OrderRef

    const sessionStorageOrderRefKey = "ActiveLogin_BankId_OrderRef";
    const sessionOrderRef = sessionStorage.getItem(sessionStorageOrderRefKey);
    sessionStorage.removeItem(sessionStorageOrderRefKey);

    // Elements

    const uiWrapperElement = <HTMLElement>document.querySelector(".activelogin-bankid-ui--wrapper");

    const statusWrapperElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-wrapper");
    const statusInfoElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-info");
    const statusSpinnerElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-spinner");
    const statusMessageElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--status-message");

    const qrCodeElement = <HTMLImageElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--qr-code-image");

    const startBankIdAppButtonElement = <HTMLButtonElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--startapp-button");
    const cancelButtonElement = <HTMLElement>uiWrapperElement.querySelector(".activelogin-bankid-ui--cancel-button");

    const uiResultForm = <HTMLFormElement>document.querySelector("form[name=activelogin-bankid-ui--result-form]");
    const uiResultInput = <HTMLInputElement>uiResultForm.querySelector("input[name=uiResult]");

    // Events

    if (sessionOrderRef) {
        document.addEventListener("DOMContentLoaded", () => {
            enableCancelButton(
                initState.antiXsrfRequestToken,
                initState.cancelReturnUrl,
                initState.protectedUiOptions,
                sessionOrderRef
            );

            showOrderRefStatus(sessionOrderRef);
        });
    } else {
        document.addEventListener("DOMContentLoaded", () => {
            resetUi();
            login();
        });
    }

    // Boot

    function showOrderRefStatus(orderRef: string) {
        showProgressStatus(configuration.initialStatusMessage);
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
    var flowIsCancelledByUser = false;
    var flowIsFinished = false;

    function enableCancelButton(requestVerificationToken: string, cancelUrl: string, protectedUiOptions: string, orderRef: string = null) {
        var onCancelButtonClick = (event: Event) => {
            cancel(requestVerificationToken, cancelUrl, protectedUiOptions, orderRef);
            event.target.removeEventListener("click", onCancelButtonClick);
        };
        cancelButtonElement.addEventListener("click", onCancelButtonClick);
    }
    function initialize(requestVerificationToken: string, returnUrl: string, cancelUrl: string, protectedUiOptions: string) {
        flowIsCancelledByUser = false;

        postJson(configuration.bankIdInitializeApiUrl,
            requestVerificationToken,
            {
                "returnUrl": returnUrl,
                "uiOptions": protectedUiOptions
            }, fetchRetryCountDefault)
            .then(data => {
                if (data.isAutoLaunch) {
                    if (!data.checkStatus) {
                        sessionStorage.setItem(sessionStorageOrderRefKey, data.orderRef);
                    }

                    if (data.deviceMightRequireUserInteractionToLaunchBankIdApp) {
                        var startBankIdAppButtonOnClick = (event: Event) => {
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

                enableCancelButton(requestVerificationToken, cancelUrl, protectedUiOptions, data.orderRef);

                showProgressStatus(configuration.initialStatusMessage);

                if (data.checkStatus) {
                    checkStatus(requestVerificationToken, returnUrl, protectedUiOptions, data.orderRef);
                }
            })
            .catch(error => {
                showErrorStatus(error.message);
                hide(qrCodeElement);
                hide(startBankIdAppButtonElement);
                enableCancelButton(requestVerificationToken, cancelUrl, protectedUiOptions);
            });
    }

    function checkStatus(requestVerificationToken: string, returnUrl: string, protectedUiOptions: string, orderRef: string) {
        if (flowIsCancelledByUser || flowIsFinished) {
            return;
        }

        postJson(configuration.bankIdStatusApiUrl,
            requestVerificationToken,
            {
                "orderRef": orderRef,
                "returnUrl": returnUrl,
                "uiOptions": protectedUiOptions,
                "autoStartAttempts": autoStartAttempts
            }, fetchRetryCountDefault)
            .then(data => {
                if (data.retryLogin) {
                    autoStartAttempts++;
                    login();
                } else if (data.isFinished) {
                    flowIsFinished = true;
                    clearTimeout(qrRefreshTimeoutId);
                    hide(qrCodeElement);

                    uiResultForm.setAttribute("action", data.redirectUri);
                    uiResultInput.value = data.result;
                    uiResultForm.submit();
                } else if (!flowIsCancelledByUser) {
                    autoStartAttempts = 0;
                    showProgressStatus(data.statusMessage);
                    setTimeout(() => {
                        checkStatus(requestVerificationToken, returnUrl, protectedUiOptions, orderRef);
                    }, 100);
                }
            })
            .catch(error => {
                clearTimeout(qrRefreshTimeoutId);
                if (!flowIsCancelledByUser) {
                    showErrorStatus(error.message);
                    hide(startBankIdAppButtonElement);
                }
                hide(qrCodeElement);
            });
    }

    function refreshQrCode(requestVerificationToken: string, qrStartState: string) {
        if (flowIsCancelledByUser || flowIsFinished || qrIsRefreshing) {
            return;
        }

        const currentTime = new Date();
        const timeSinceLastRefresh = currentTime.getTime() - qrLastRefreshTimestamp.getTime();
        if (timeSinceLastRefresh < configuration.qrCodeRefreshIntervalMs) {
            qrRefreshTimeoutId = setTimeout(() => {
                refreshQrCode(requestVerificationToken, qrStartState);
            }, configuration.qrCodeRefreshIntervalMs);
            return;
        }
        qrIsRefreshing = true;

        postJson(configuration.bankIdQrCodeApiUrl,
            requestVerificationToken,
            {
                "qrStartState": qrStartState
            }, fetchRetryCountDefault)
            .then(data => {
                if (!!data.qrCodeAsBase64) {
                    qrLastRefreshTimestamp = new Date();
                    setQrCode(data.qrCodeAsBase64);
                    qrRefreshTimeoutId = setTimeout(() => {
                        refreshQrCode(requestVerificationToken, qrStartState);
                    }, configuration.qrCodeRefreshIntervalMs);
                }
            })
            .catch(error => {
                if (flowIsFinished) {
                    return;
                }

                if (!flowIsCancelledByUser) {
                    showErrorStatus(error.message);
                    hide(startBankIdAppButtonElement);
                }
                hide(qrCodeElement);
            })
            .finally(() => {
                qrIsRefreshing = false;
            });
    }

    function setQrCode(qrCodeAsBase64: string) {
        qrCodeElement.src = 'data:image/png;base64, ' + qrCodeAsBase64;
        show(qrCodeElement);
    }

    function cancel(requestVerificationToken: string, cancelReturnUrl: string, protectedUiOptions: string, orderRef: string = null) {
        flowIsCancelledByUser = true;

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

    function postJson(url: string, requestVerificationToken: string, data: any, retryCount: number = 0): Promise<any> {
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
            .catch(error => {
                if (retryCount > 0) {
                    return delay(fetchRetryDelayMs).then(() => {
                        return postJson(url, requestVerificationToken, data, retryCount - 1);
                    });
                }

                throw error;
            })
            .then(response => {
                if (!response.ok && retryCount > 0) {
                    return delay(fetchRetryDelayMs).then(() => {
                        return postJson(url, requestVerificationToken, data, retryCount - 1)
                    });
                }

                return response;
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

    function showProgressStatus(status: string) {
        showStatus(status, "progress", true);
    }

    function showErrorStatus(status: string) {
        showStatus(status, "error", false);
    }

    function showStatus(status: string, statusType: string, spinner: boolean) {
        statusInfoElement.className = `activelogin-bankid-ui--status-info activelogin-bankid-ui--status-info--${statusType}`;
        statusMessageElement.innerText = status;
        setVisibility(statusSpinnerElement, spinner, "inline-block");
        show(statusWrapperElement);
    }

    function setVisibility(element: HTMLElement, visible: boolean, display: string = null) {
        if (visible) {
            show(element, display);
        } else {
            hide(element);
        }
    }

    function show(element: HTMLElement, display: string = "block") {
        if (!element) {
            return;
        }

        element.style.display = display;
    }

    function hide(element: HTMLElement) {
        if (!element) {
            return;
        }

        element.style.display = "none";
    }

    function delay(timeout: number) {
        return new Promise(resolve => setTimeout(resolve, timeout));
    }
}

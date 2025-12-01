class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    static jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    static jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

document.addEventListener("submit", (e) => {
    const form = e.target;

    if (form && form["id"] === "signup-form") {
        e.preventDefault();                        // Перехоплення надсилання форми та переведення 
        // його до асинхронної форми (AJAX)
        fetch("/User/Register", {                  //
            method: "POST",                        //
            body: new FormData(form)               //
        }).then(r => {                             //
            return r.json();
        }).then(j => {
            if (j.status === "Ok") {
                alert("Вітаємо з успішною реєстрацією");
            }
            else {
                for (let elem of form.querySelectorAll("input")) {
                    elem.classList.remove("is-invalid");
                    elem.classList.add("is-valid")
                }

                for (let name in j['errors']) {
                    let input = form.querySelector(`[name="${name}"]`);
                    if (input) {
                        input.classList.add("is-invalid");
                        let fb = form.querySelector(`[name=${name}]+.invalid-feedback`);
                        if (fb) {
                            fb.innerText = j['errors'][name];
                        }
                    }
                    else {
                        console.error(`input name = '${name}' not found`)
                    }
                }
            }
        })
    }

    if (form && form["id"] == "auth-form") {
        e.preventDefault();
        const formData = new FormData(form);

        const login = formData.get("user-login");
        const password = formData.get("user-password");

        let invalid = login.length === 0;
        const loginCont = form.querySelector("div:nth-child(1)");

        if (invalid) {
            let loginInvalid = loginCont.querySelector(".invalid-feedback")

            if (!loginInvalid) {
                loginInvalid = document.createElement("div");
                loginInvalid.textContent = "Необхідно зазначити логін!";
                loginInvalid.classList.add("invalid-feedback");

                loginCont.append(loginInvalid);
            }
            loginCont.querySelector("input").classList.add("is-invalid");
            loginCont.querySelector("input").classList.remove("is-valid");
            return;
        } else {
            loginCont.querySelector("input").classList.remove("is-invalid")
            loginCont.querySelector("input").classList.add("is-valid");
        }

        invalid = password.length === 0;
        const passwordCont = form.querySelector("div:nth-child(2)");

        if (invalid) {
            let passwordInvalid = passwordCont.querySelector(".invalid-feedback")

            if (!passwordInvalid) {
                passwordInvalid = document.createElement("div");
                passwordInvalid.textContent = "Необхідно зазначити пароль!";
                passwordInvalid.classList.add("invalid-feedback");

                passwordCont.append(passwordInvalid);
            }
            passwordCont.querySelector("input").classList.add("is-invalid");
            passwordCont.querySelector("input").classList.remove("is-valid");
            return;
        } else {
            passwordCont.querySelector("input").classList.remove("is-invalid")
            passwordCont.querySelector("input").classList.add("is-valid");
        }


        // RFC 7617
        // https://datatracker.ietf.org/doc/html/rfc7617

        const userPass = login + ":" + password
        const basicCredentials = Base64.encode(userPass)
        const header = "Authorization: Basic " + basicCredentials

        fetch("/User/Authenticate", {
            method: "GET",
            headers: {
                "Authorization": "Basic " + basicCredentials
            }
        }).then(r => {
            if (r.status >= 400) {
                r.text().then((error) => {
                    if (error !== null) {
                        let modalFooter = document.querySelector(".modal-footer");
                        let errorCont = modalFooter.querySelector(".alert-danger");

                        if (error === "") {
                            errorCont.remove();
                        } else {
                            if (errorCont) {
                                errorCont.remove();
                            }

                            errorCont = document.createElement("div");
                            errorCont.classList.add("alert");
                            errorCont.classList.add("alert-danger");
                            errorCont.classList.add("text-truncate");
                            errorCont.style.width = "fit-content"
                            errorCont.style.maxWidth = "250px"
                            errorCont.style.padding = "6px"
                            errorCont.title = error;
                            errorCont.textContent = error;

                            modalFooter.prepend(errorCont);
                        }

                    }
                })
            }
            else {
                window.location.reload();
            }
        })
    }
})

document.addEventListener("DOMContentLoaded", () => {
    let btn = document.getElementById("btn-profile-edit");
    if (btn) btn.addEventListener('click', btnProfileEditClick)
})

function btnProfileEditClick() {
    for (let item of document.querySelectorAll("[data-profile-editable]")) {
        item.setAttribute("contenteditable", true)
    }
}

function btnProfileEditClick(e) {
    const btn = e.target.closest("button");

    if (btn.isActived) {
        btn.isActived = false;
        btn.classList.remove("btn-outline-success");
        btn.classList.add("btn-outline-warning");
        let changes = {};

        for (let item of document.querySelectorAll("[data-profile-editable]")) {
            item.removeAttribute("contenteditable")
            let currentText = item.innerText.replace(/(\r\n|\n|\r)/gm, "").trim();
            if (item.initialText !== currentText) {
                changes[item.getAttribute("data-profile-editable")] = item.innerText;
            }
            let tr = item.closest("[data-profile-hidden]");
            if (tr) {
                tr.style.display = "none";
            }
        }

        if (Object.keys(changes).length > 0) {
            fetch("/user/update", {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(changes)
            })
                .then(r => {
                    if (r.status === 202) {
                        window.location.reload();
                    }
                    else {
                        r.text().then(alert);
                    }
                });
        }
    }
    else {
        btn.isActived = true;
        btn.classList.remove("btn-outline-warning")
        btn.classList.add("btn-outline-success");
        for (let item of document.querySelectorAll("[data-profile-editable]")) {
            item.setAttribute("contenteditable", true)
            item.initialText = item.innerText;

            let tr = item.closest("[data-profile-hidden]");
            if (tr) {
                tr.style.display = "table-row";
            }
        }
    }
}


/*
Base64

"ABC" =>
    A       B        C
01000001 01000010 01000011
      |     |      |
010000  010100 001001 000011 - з таблиці Base64
    Q       U       J      D

*/
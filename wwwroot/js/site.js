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
                r.text().then(alert);
            }
        })
    }
})


/*
Base64

"ABC" =>
    A       B        C
01000001 01000010 01000011
      |     |      |
010000  010100 001001 000011 - з таблиці Base64
    Q       U       J      D

*/
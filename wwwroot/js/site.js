document.addEventListener("submit", (e) => {
    const form = e.target;

    if (form && form["id"] == "auth-form") {
        e.preventDefault();
        const formData = new FormData(form);

        const login = formData.get("user-login");
        const password = formData.get("user-password");
        console.log(`${login}: ${password}`)

        // RFC 6717
        // https://datatracker.ietf.org/doc/html/rfc7617

        // userPass = login + ":" + password
        // basicCredentials = Base64.encode(userPass)
        // header Authorization = Basic basicCredentials

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
        } else {
            passwordCont.querySelector("input").classList.remove("is-invalid")
            passwordCont.querySelector("input").classList.add("is-valid");
        }
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
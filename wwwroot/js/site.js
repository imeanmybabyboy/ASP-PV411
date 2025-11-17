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
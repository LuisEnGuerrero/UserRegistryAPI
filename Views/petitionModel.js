fetch("http://localhost:5001/api/user", {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify({
        name: "Luis Enrique Guerrero Ibarra",
        phone: "3208172936",
        address: "Calle 70A Sur # 17M - 29",
        countryId: 46,
        departmentId: 1127,
        municipalityId: 149
    })
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error("Error:", error));


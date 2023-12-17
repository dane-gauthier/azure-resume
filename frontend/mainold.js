window.addEventListener('DOMContentLoaded', (event) => {
    getVisitCount().then(count => {
        // Do something with the count if needed
    });
});

const functionApi = 'http://localhost:7071/api/AzureResumeCounter';

const getVisitCount = () => {
    return new Promise((resolve, reject) => {
        let count = 30;
        fetch(functionApi)
            .then(response => {
                return response.json();
            })
            .then(response => {
                console.log("Website call function API.");
                count = response.count;
                document.getElementById("counter").innerText = count;
                resolve(count);
            })
            .catch(error => {
                console.log(error);
                reject(error);
            });
    });
};

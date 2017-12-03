var app = angular.module("foodApp", []);

app.controller("myCtrl", function ($scope) {
    $scope.test = "FOOD APP IS WORKING";

    $scope.imageUpload = function(event) {

        console.log("IMAGE UPLOAD, EVENT => ");
        console.log(event);

        if (event.target.files && event.target.files[0]) {
            let reader = new FileReader();
            let fileInfo = event.target.files[0];

            // CHECK IMAGE SIZE
            if (fileInfo.size > this.AVATAR_MAX_SIZE) {
                alert("WOW MAN! THE SELECTED IMAGE IS TOOOO BIG....\nMAX ALLOWED SIZE IS 1 MB!");
                return;
            }

            console.log('file type');
            console.log(fileInfo);
            // CHECK FILE TYPE
            if(!fileInfo.type.includes('image') ){
                alert("NOT IMAGE! Ai ai ai.....");
                return;
            }
            else{
                console.log('OK IMAGE TYPE');
            }

            reader.onload = function(event) {
                var imageAsBase64 = event.target.result;
                console.log("WE HAVE IMAGE !!!! ===> ");
                console.log(imageAsBase64);
            }

            reader.readAsDataURL(event.target.files[0]);
        }
    };
});
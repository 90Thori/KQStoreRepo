
function isValidEmail(email) {
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
}

function isValidPhone(phone) {
    var phonePattern = /^\d{10}$/;
    return phonePattern.test(phone);
}

function isValidPassword(password) {
    var passwordPattern = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/;
    return passwordPattern.test(password);
}

function validateEmail() {
    var email = $("#register-email").val();
    if (!isValidEmail(email)) {
        $("#email-error").html("Email không đúng định dạng.");
        $("#email-error").show();
    } else {
        $("#email-error").hide();
    }
}

function validatePhone() {
    var phone = $("#register-phone").val();
    if (!isValidPhone(phone)) {
        $("#phone-error").html("Số điện thoại phải có đủ 10 số.");
        $("#phone-error").show();
    } else {
        $("#phone-error").hide();
    }
}

function validatePassword() {
    var password = $("#register-password").val();
    if (!isValidPassword(password)) {
        $("#password-error").html("Mật khẩu phải có ít nhất 6 ký tự gồm chữ và số.");
        $("#password-error").show();
    } else {
        $("#password-error").hide();
    }
}

function validateConfirmPassword() {
    var password = $("#register-password").val();
    var confirmPassword = $("#register-confirm-password").val();
    if (password !== confirmPassword) {
        $("#confirm-password-error").html("Mật khẩu xác nhận không khớp.");
        $("#confirm-password-error").show();
    } else {
        $("#confirm-password-error").hide();
    }
}

function validateUsername() {
    var username = $("#register-username").val();
    $.ajax({
        type: "POST",
        url: "/Home/ValidateUsername",
        data: {
            username: username,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        dataType: "json",
        success: function (response) {
            if (!response.isValid) {
                $("#username-error").html("Tài khoản đã tồn tại.");
                $("#username-error").show();
            } else {
                $("#username-error").hide();
            }
        }
    });
}

function register() {
    // Reset all error fields
    validateEmail();
    validatePhone();
    validatePassword();
    validateConfirmPassword();
    validateUsername();

    // Check if there are any visible errors
    if ($(".error:visible").length === 0) {
        // Perform the registration if there are no errors
        var name = $("#register-name").val();
        var username = $("#register-username").val();
        var password = $("#register-password").val();
        var confirmPassword = $("#register-confirm-password").val(); // Thêm confirmPassword
        var phone = $("#register-phone").val();
        var email = $("#register-email").val();

        $.ajax({
            type: "POST",
            url: "/Home/Register",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                Name: name,
                username: username,
                password: password,
                confirmPassword: confirmPassword, // Gửi confirmPassword
                phone: phone,
                email: email
            },
            dataType: "json",
            success: function (response) {
                console.log(response); // Log phản hồi để kiểm tra
                if (response.success) {
                    window.location.href = response.redirectTo;
                } else {
                    if (response.errors) {
                        response.errors.forEach(function (error) {
                            var errorField = "#" + error.Key.toLowerCase() + "-error";
                            $(errorField).html(error.ErrorMessage);
                            $(errorField).show();
                        });
                    } else {
                        $("#register-error").html(response.errorMessage);
                        $("#register-error").show();
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Đã xảy ra lỗi khi đăng ký:", status, error); // Log lỗi
                alert("Đã xảy ra lỗi khi đăng ký.");
            }
        });
    }
}


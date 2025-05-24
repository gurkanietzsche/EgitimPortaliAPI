// User Operations JavaScript

// Sayfa yüklendiğinde çalışacak fonksiyon
$(document).ready(function () {
    // Kullanıcı yönetimi sayfasındaysa
    if (window.location.pathname.includes("UserManagement")) {
        loadUsers();
        setupUserEvents();
    }
});

// Kullanıcıları yükleyen fonksiyon
function loadUsers() {
    $.ajax({
        url: '/Home/GetUsers',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                displayUsers(response.data);
            } else {
                showAlert('danger', 'Kullanıcılar yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kullanıcı detaylarını yükleyen fonksiyon
function loadUserDetails(userId) {
    $.ajax({
        url: '/Home/GetUser',
        type: 'GET',
        data: { id: userId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                populateUserEditForm(response.data);
            } else {
                showAlert('danger', 'Kullanıcı detayları yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Yeni kullanıcı ekleyen fonksiyon
function addUser(userData) {
    $.ajax({
        url: '/Home/AddUser',
        type: 'POST',
        data: userData,
        success: function (response) {
            if (response.success) {
                showAlert('success', 'Kullanıcı başarıyla eklendi.');
                loadUsers();
                $('#addUserModal').modal('hide');
            } else {
                showAlert('danger', 'Kullanıcı eklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kullanıcı güncelleyen fonksiyon
function updateUser(userData) {
    $.ajax({
        url: '/Home/UpdateUser',
        type: 'POST',
        data: userData,
        success: function (response) {
            if (response.success) {
                showAlert('success', 'Kullanıcı başarıyla güncellendi.');
                loadUsers();
                $('#editUserModal').modal('hide');
            } else {
                showAlert('danger', 'Kullanıcı güncellenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kullanıcı silen fonksiyon
function deleteUser(userId) {
    if (confirm('Bu kullanıcıyı silmek istediğinizden emin misiniz?')) {
        $.ajax({
            url: '/Home/DeleteUser',
            type: 'POST',
            data: { id: userId },
            success: function (response) {
                if (response.success) {
                    showAlert('success', 'Kullanıcı başarıyla silindi.');
                    loadUsers();
                } else {
                    showAlert('danger', 'Kullanıcı silinirken hata oluştu: ' + response.message);
                }
            },
            error: function (error) {
                showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
            }
        });
    }
}

// Kullanıcı listesini gösteren fonksiyon
function displayUsers(users) {
    var container = $('#usersContainer');
    container.empty();

    if (users && users.length > 0) {
        var table = $('<table class="table table-striped">').append(
            '<thead>' +
            '<tr>' +
            '<th>ID</th>' +
            '<th>Ad</th>' +
            '<th>Soyad</th>' +
            '<th>Email</th>' +
            '<th>Rol</th>' +
            '<th>İşlemler</th>' +
            '</tr>' +
            '</thead>'
        );
        var tbody = $('<tbody>');

        $.each(users, function (i, user) {
            var row = $('<tr>').append(
                '<td>' + user.id + '</td>' +
                '<td>' + user.firstName + '</td>' +
                '<td>' + user.lastName + '</td>' +
                '<td>' + user.email + '</td>' +
                '<td>' + (user.role || 'Belirtilmemiş') + '</td>' +
                '<td>' +
                '<button class="btn btn-sm btn-primary edit-user" data-id="' + user.id + '">Düzenle</button> ' +
                '<button class="btn btn-sm btn-danger delete-user" data-id="' + user.id + '">Sil</button>' +
                '</td>'
            );
            tbody.append(row);
        });

        table.append(tbody);
        container.append(table);
    } else {
        container.append('<div class="alert alert-info">Henüz kullanıcı bulunmamaktadır.</div>');
    }
}

// Kullanıcı düzenleme formunu dolduran fonksiyon
function populateUserEditForm(user) {
    $('#editUserId').val(user.id);
    $('#editFirstName').val(user.firstName);
    $('#editLastName').val(user.lastName);
    $('#editEmail').val(user.email);
    $('#editRole').val(user.role);
    $('#editUserModal').modal('show');
}

// Kullanıcı işlemlerinin eventlerini ayarlayan fonksiyon
function setupUserEvents() {
    // Yeni kullanıcı ekleme formu submit eventi
    $('#addUserForm').on('submit', function (e) {
        e.preventDefault();
        var userData = {
            firstName: $('#addFirstName').val(),
            lastName: $('#addLastName').val(),
            email: $('#addEmail').val(),
            password: $('#addPassword').val(),
            role: $('#addRole').val()
        };
        addUser(userData);
    });

    // Kullanıcı düzenleme formu submit eventi
    $('#editUserForm').on('submit', function (e) {
        e.preventDefault();
        var userData = {
            id: $('#editUserId').val(),
            firstName: $('#editFirstName').val(),
            lastName: $('#editLastName').val(),
            email: $('#editEmail').val(),
            role: $('#editRole').val()
        };
        updateUser(userData);
    });

    // Dynamically added button events using delegation
    $(document).on('click', '.edit-user', function () {
        var userId = $(this).data('id');
        loadUserDetails(userId);
    });

    $(document).on('click', '.delete-user', function () {
        var userId = $(this).data('id');
        deleteUser(userId);
    });
}

// Uyarı mesajı gösteren yardımcı fonksiyon
function showAlert(type, message) {
    var alertBox = $('<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>');

    $('#alertContainer').append(alertBox);

    // 5 saniye sonra otomatik kapat
    setTimeout(function () {
        alertBox.alert('close');
    }, 5000);
}
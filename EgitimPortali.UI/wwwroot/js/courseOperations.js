// Course Operations JavaScript

// Sayfa yüklendiğinde çalışacak fonksiyon
$(document).ready(function () {
    // Kurs yönetimi sayfasındaysa
    if (window.location.pathname.includes("CourseManagement")) {
        loadCategories();
        loadCourses();
        setupCourseEvents();
    }
    // Kurslar sayfasındaysa
    else if (window.location.pathname.includes("Courses")) {
        loadCourses();
    }
    // Kurs detay sayfasındaysa
    else if (window.location.pathname.includes("CourseDetail")) {
        var courseId = $("#courseDetailContainer").data("course-id");
        if (courseId) {
            loadCourseDetails(courseId);
        }
    }
});

// Kurs listesini yükleyen fonksiyon
function loadCourses() {
    $.ajax({
        url: '/Home/GetCourses',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                displayCourses(response.data);
            } else {
                showAlert('danger', 'Kurslar yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kurs detaylarını yükleyen fonksiyon
function loadCourseDetails(courseId) {
    $.ajax({
        url: '/Home/GetCourse',
        type: 'GET',
        data: { id: courseId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                displayCourseDetails(response.data);
            } else {
                showAlert('danger', 'Kurs detayları yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Yeni kurs ekleyen fonksiyon
function addCourse(courseData) {
    $.ajax({
        url: '/Home/AddCourse',
        type: 'POST',
        data: courseData,
        success: function (response) {
            if (response.success) {
                showAlert('success', 'Kurs başarıyla eklendi.');
                loadCourses();
                $('#addCourseModal').modal('hide');
            } else {
                showAlert('danger', 'Kurs eklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kurs güncelleyen fonksiyon
function updateCourse(courseData) {
    $.ajax({
        url: '/Home/UpdateCourse',
        type: 'POST',
        data: courseData,
        success: function (response) {
            if (response.success) {
                showAlert('success', 'Kurs başarıyla güncellendi.');
                loadCourses();
                $('#editCourseModal').modal('hide');
            } else {
                showAlert('danger', 'Kurs güncellenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kurs silen fonksiyon
function deleteCourse(courseId) {
    if (confirm('Bu kursu silmek istediğinizden emin misiniz?')) {
        $.ajax({
            url: '/Home/DeleteCourse',
            type: 'POST',
            data: { id: courseId },
            success: function (response) {
                if (response.success) {
                    showAlert('success', 'Kurs başarıyla silindi.');
                    loadCourses();
                } else {
                    showAlert('danger', 'Kurs silinirken hata oluştu: ' + response.message);
                }
            },
            error: function (error) {
                showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
            }
        });
    }
}

// Kursa kayıt olma fonksiyonu
function enrollCourse(courseId) {
    $.ajax({
        url: '/Home/EnrollCourse',
        type: 'POST',
        data: { courseId: courseId },
        success: function (response) {
            if (response.success) {
                showAlert('success', response.message);
            } else {
                showAlert('danger', response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kurs listesini gösteren fonksiyon (örnek)
function displayCourses(courses) {
    var container = $('#coursesContainer');
    container.empty();

    if (courses && courses.length > 0) {
        // Admin paneli için kurs yönetimi tablosu
        if (window.location.pathname.includes("CourseManagement")) {
            var table = $('<table class="table table-striped">').append(
                '<thead>' +
                '<tr>' +
                '<th>ID</th>' +
                '<th>Başlık</th>' +
                '<th>Açıklama</th>' +
                '<th>Fiyat</th>' +
                '<th>Kategori</th>' +
                '<th>İşlemler</th>' +
                '</tr>' +
                '</thead>'
            );
            var tbody = $('<tbody>');

            $.each(courses, function (i, course) {
                var row = $('<tr>').append(
                    '<td>' + course.id + '</td>' +
                    '<td>' + course.title + '</td>' +
                    '<td>' + (course.description.length > 50 ? course.description.substring(0, 50) + '...' : course.description) + '</td>' +
                    '<td>' + course.price + ' TL</td>' +
                    '<td>' + (course.category ? course.category.name : 'Kategori Yok') + '</td>' +
                    '<td>' +
                    '<button class="btn btn-sm btn-primary edit-course" data-id="' + course.id + '">Düzenle</button> ' +
                    '<button class="btn btn-sm btn-danger delete-course" data-id="' + course.id + '">Sil</button>' +
                    '</td>'
                );
                tbody.append(row);
            });

            table.append(tbody);
            container.append(table);
        }
        // Normal kullanıcılar için kurs kartları
        else {
            var row = $('<div class="row">');

            $.each(courses, function (i, course) {
                var card = $(
                    '<div class="col-md-4 mb-4">' +
                    '<div class="card h-100">' +
                    '<img src="' + (course.imageUrl || '/img/default-course.jpg') + '" class="card-img-top" alt="' + course.title + '">' +
                    '<div class="card-body">' +
                    '<h5 class="card-title">' + course.title + '</h5>' +
                    '<p class="card-text">' + (course.description.length > 100 ? course.description.substring(0, 100) + '...' : course.description) + '</p>' +
                    '<p class="card-text"><small class="text-muted">Kategori: ' + (course.category ? course.category.name : 'Kategori Yok') + '</small></p>' +
                    '<p class="card-text"><strong>Fiyat: ' + course.price + ' TL</strong></p>' +
                    '<a href="/Home/CourseDetail/' + course.id + '" class="btn btn-primary">Detaylar</a> ' +
                    '<button class="btn btn-success enroll-course" data-id="' + course.id + '">Kaydol</button>' +
                    '</div>' +
                    '</div>' +
                    '</div>'
                );
                row.append(card);
            });

            container.append(row);
        }
    } else {
        container.append('<div class="alert alert-info">Henüz kurs bulunmamaktadır.</div>');
    }
}

// Kurs detaylarını gösteren fonksiyon (örnek)
function displayCourseDetails(course) {
    var container = $('#courseDetailContainer');
    container.empty();

    if (course) {
        var content = $(
            '<div class="row">' +
            '<div class="col-md-8">' +
            '<h1>' + course.title + '</h1>' +
            '<p><strong>Kategori:</strong> ' + (course.category ? course.category.name : 'Belirtilmemiş') + '</p>' +
            '<p><strong>Zorluk:</strong> ' + (course.difficultyLevel || 'Belirtilmemiş') + '</p>' +
            '<p><strong>Süre:</strong> ' + (course.duration || '0') + ' saat</p>' +
            '<hr>' +
            '<h3>Kurs Açıklaması</h3>' +
            '<p>' + course.description + '</p>' +
            '</div>' +
            '<div class="col-md-4">' +
            '<div class="card">' +
            '<img src="' + (course.imageUrl || '/img/default-course.jpg') + '" class="card-img-top" alt="' + course.title + '">' +
            '<div class="card-body">' +
            '<h5 class="card-title">Fiyat: ' + course.price + ' TL</h5>' +
            '<button id="enrollButton" class="btn btn-success btn-block">Kursa Kaydol</button>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

        container.append(content);

        // Kursa kayıt olma butonu eventi
        $('#enrollButton').on('click', function () {
            enrollCourse(course.id);
        });
    } else {
        container.append('<div class="alert alert-danger">Kurs bulunamadı.</div>');
    }
}

// Kategorileri yükleyen fonksiyon
function loadCategories() {
    $.ajax({
        url: '/Home/GetCategories',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Kategori dropdown'larını doldur
                populateCategoryDropdowns(response.data);
            } else {
                showAlert('danger', 'Kategoriler yüklenirken hata oluştu: ' + response.message);
            }
        },
        error: function (error) {
            showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
        }
    });
}

// Kategori dropdown'larını dolduran fonksiyon
function populateCategoryDropdowns(categories) {
    var addDropdown = $('#addCategoriesList');
    var editDropdown = $('#editCategoriesList');

    addDropdown.empty();
    editDropdown.empty();

    $.each(categories, function (i, category) {
        addDropdown.append($('<option>', {
            value: category.id,
            text: category.name
        }));

        editDropdown.append($('<option>', {
            value: category.id,
            text: category.name
        }));
    });
}

// Kurs işlemlerinin eventlerini ayarlayan fonksiyon
function setupCourseEvents() {
    // Yeni kurs ekleme formu submit eventi
    $('#addCourseForm').on('submit', function (e) {
        e.preventDefault();
        var courseData = {
            title: $('#addTitle').val(),
            description: $('#addDescription').val(),
            price: $('#addPrice').val(),
            categoryId: $('#addCategoriesList').val(),
            imageUrl: $('#addImageUrl').val(),
            duration: $('#addDuration').val(),
            difficultyLevel: $('#addDifficultyLevel').val()
        };
        addCourse(courseData);
    });

    // Kurs düzenleme formu submit eventi
    $('#editCourseForm').on('submit', function (e) {
        e.preventDefault();
        var courseData = {
            id: $('#editCourseId').val(),
            title: $('#editTitle').val(),
            description: $('#editDescription').val(),
            price: $('#editPrice').val(),
            categoryId: $('#editCategoriesList').val(),
            imageUrl: $('#editImageUrl').val(),
            duration: $('#editDuration').val(),
            difficultyLevel: $('#editDifficultyLevel').val()
        };
        updateCourse(courseData);
    });

    // Dynamically added button events using delegation
    $(document).on('click', '.edit-course', function () {
        var courseId = $(this).data('id');
        // Kurs bilgilerini getir ve modala doldur
        $.ajax({
            url: '/Home/GetCourse',
            type: 'GET',
            data: { id: courseId },
            success: function (response) {
                if (response.success) {
                    var course = response.data;
                    $('#editCourseId').val(course.id);
                    $('#editTitle').val(course.title);
                    $('#editDescription').val(course.description);
                    $('#editPrice').val(course.price);
                    $('#editCategoriesList').val(course.categoryId);
                    $('#editImageUrl').val(course.imageUrl);
                    $('#editDuration').val(course.duration);
                    $('#editDifficultyLevel').val(course.difficultyLevel);
                    $('#editCourseModal').modal('show');
                } else {
                    showAlert('danger', 'Kurs bilgileri alınamadı: ' + response.message);
                }
            },
            error: function (error) {
                showAlert('danger', 'API bağlantısı hatası: ' + error.statusText);
            }
        });
    });

    $(document).on('click', '.delete-course', function () {
        var courseId = $(this).data('id');
        deleteCourse(courseId);
    });

    $(document).on('click', '.enroll-course', function () {
        var courseId = $(this).data('id');
        enrollCourse(courseId);
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
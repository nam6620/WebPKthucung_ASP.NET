const load = document.querySelector.bind(document);
const loadNhieu = document.querySelectorAll.bind(document);
const player = load(".app");
const profile_img = load(".header_right_img");
const profile_expand = load(".header_right_img_expand");
const phoneNumber = load("#phoneNumber");
const app = {
    navRightIndex: false,
    handleEvents: function () {
        _this = this;
        //Xử lý khi click vào img profile
        profile_img.onclick = () => {
            if (!_this.navRightIndex) {
                _this.navRightIndex = true;
                profile_expand.style.display = "block";
            } else {
                _this.navRightIndex = false;
                profile_expand.style.display = "none";
            }
        };
    },
    start: function () {
        // lắng nghe sự kiện
        this.handleEvents();
    },
};
app.start();

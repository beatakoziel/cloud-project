<template>
  <div>
    <div class="tile">
      <div @click="setFile">
        <i class="fas fa-cloud-download-alt download-ico "></i>
      </div>
      <div class="icon d-flex justify-content-center">
        <div v-if="fileType === 'application/pdf'">
          <i class="fas fa-file-pdf"></i>
        </div>
        <div
          v-else-if="
            fileType === 'image/jpg' ||
              fileType === 'image/png' ||
              fileType === 'image/jpeg'
          "
        >
          <i class="fas fa-file-image"></i>
        </div>
        <div v-else-if="fileType === 'audio/mp3'">
          <i class="fas fa-file-audio"></i>
        </div>
        <div v-else>
          <i class="fas fa-file"></i>
        </div>
      </div>
      <div class="name d-flex justify-content-center pl-2 pr-2">
        {{ this.fileName }}
      </div>
    </div>
  </div>
</template>
<script>
import { mapActions } from "vuex";
const STORE = "HomeStore";
export default {
  props: {
    fileId: {
      type: String,
      required: true,
    },
    fileName: {
      type: String,
      required: true,
    },
    fileType: {
      type: String,
      required: true,
    },
  },
  methods: {
    ...mapActions(STORE, ["downloadFile"]),
    setFile() {
      this.downloadFile(this.fileId).then((response) => {
        this.download(response);
      });
    },
    download(source) {
      const byteCharacters = atob(source);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);

      let uriContent = URL.createObjectURL(
        new Blob([byteArray], { type: `${this.fileType}` })
      );
      let link = document.createElement("a");
      link.setAttribute("href", uriContent);
      link.setAttribute("download", this.fileName);
      let event = new MouseEvent("click");
      link.dispatchEvent(event);
    },
  },
};
</script>
<style scoped>
.tile {
  height: 160px;
  width: 160px;
  border: 1px solid black;
  border-radius: 5px;
  cursor: pointer;
}
.icon {
  width: 150px;
  margin-top: 4px;
  color: #000;
  font-size: 64px;
  justify-content: center;
  margin-left: auto;
  margin-right: auto;
}
.name {
  font-size: 16px;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.download-ico {
  text-align: right;
  cursor: pointer;
  font-size: 20px;
}
.download-ico:hover {
  color: blue;
}
</style>

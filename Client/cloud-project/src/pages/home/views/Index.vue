<template>
  <div>
    <div v-if="this.$route.params.dirId === `0`" class="row mb-4">
      <h1>Folder główny</h1>
    </div>
    <div v-else>
      <h1>getFolderName TODO</h1>
    </div>
    <div class="row">
      <h4>Foldery</h4>
      <div class="mt-2">
        <button class="button-wide" @click="addDirectory($event)">
          Dodaj folder
        </button>
      </div>
      <div class="mt-2 flexxin" v-if="getDirectoriesList.length !== 0">
        <div class="mr-2" v-for="dir in getDirectoriesList" :key="dir.id">
          <div @click="routerPushToFolder(dir.id)">
            <i class="fas fa-folder"></i>
            {{ dir.name }}
          </div>
        </div>
      </div>
      <div v-else class="no-files">
        Nie masz żadnych folderów
      </div>
      <b-modal ref="addDirModal" hide-footer>
        <div class="d-block text-center mb-3">
          <h4>Dodaj folder</h4>
          <input v-model="dirName" />
        </div>
        <div class="text-center">
          <b-button variant="outline-primary" @click="hideAddDirModalOnCancel"
            >Anuluj</b-button
          >
          <b-button
            class="ml-2"
            variant="outline-danger"
            @click="hideAddDirModalOnProceed"
            >Zapisz</b-button
          >
        </div>
      </b-modal>
    </div>
    <div class="row mt-4">
      <h4>Pliki</h4>
      <div class="mb-4">
        <input type="file" ref="input" v-on:change="getFile($event)" />
        <button class="button" @click="submitFile($event)">Prześlij</button>
      </div>
      <div class="flexxin">
        <div
          v-for="file in getFilesList"
          v-bind:key="file.id"
          class="mr-4 mb-4"
        >
          <div class="flexxin">
            <div @click="download(file.id, file.name)">
              <i class="fas fa-cloud-download-alt download-ico mr-1"></i>
            </div>
            <div @click="removeFile(file)">
              <i class="fas fa-trash-alt trash mr-1"></i>
            </div>
            <div @click="showHistory(file)">
              <i class="fas fa-history versions"></i>
            </div>
          </div>
          <FileTile
            :fileId="file.id"
            :fileType="file.contentType"
            :fileName="file.name"
          />
        </div>
        <div v-if="this.filesList.length === 0" class="no-files">
          Nie masz żadnych plików
        </div>

        <LoadSpinner v-if="loading" />
      </div>
      <b-modal ref="delModal" hide-footer>
        <div class="d-block text-center mb-3">
          <h4>Czy na pewno usunąć {{ fileToDelete.name }}?</h4>
        </div>
        <div class="text-center">
          <b-button variant="outline-primary" @click="hideModalOnCancel"
            >Anuluj</b-button
          >
          <b-button
            class="ml-2"
            variant="outline-danger"
            @click="hideModalOnProceed"
            >Potwierdź</b-button
          >
        </div>
      </b-modal>
      <b-modal ref="prevModal" hide-footer title="Poprzednie wersje">
        <div v-if="this.previousVersions.length !== 0">
          <div v-for="prev in this.previousVersions" :key="prev.id">
            <div class="flexxin">
              Z dnia: {{ prev.createdDate | moment("DD.MM.YYYY, h:mm:ss a") }}
              <div @click="download(prev.id, prev.name)">
                <i class="fas fa-cloud-download-alt download-ico ml-2"></i>
              </div>
            </div>
          </div>
        </div>
        <div v-else-if="this.previousVersions == []">
          Nie ma poprzednich wersji pliku
        </div>
        <div class="text-center mt-4">
          <b-button variant="outline-primary" @click="hidePreviousVersionsModal"
            >Zamknij</b-button
          >
        </div>
      </b-modal>
      <div v-if="this.$route.params.dirId !== `0`">
        <button @click="goBack">Powrót</button>
      </div>
    </div>
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";
import { mapFields } from "vuex-map-fields";

import FileTile from "../components/FileTile";
import LoadSpinner from "../components/LoadSpinner";

const STORE = "HomeStore";

export default {
  name: "home",
  data() {
    return {
      fileToDelete: {
        name: "",
        id: "",
      },
      newFile: null,
      loading: false,
    };
  },
  computed: {
    ...mapFields(STORE, [
      "filesList",
      "file.source",
      "file.name",
      "file.type",
      "previousVersions",
      "directory.dirName",
    ]),
    ...mapGetters(STORE, ["getFilesList", "getDirectoriesList"]),
  },
  methods: {
    ...mapActions(STORE, [
      "setFilesList",
      "uploadFile",
      "deleteFile",
      "downloadFile",
      "addDir",
      "setDirectoriesList",
    ]),
    getFile(event) {
      this.newFile = event.target.files[0];
    },
    async submitFile(event) {
      event.preventDefault();
      let formData = new FormData();
      formData.append("file", this.newFile);
      this.loading = true;
      await this.uploadFile(formData);
      await this.setFilesList();
      this.loading = false;
    },
    showHistory(file) {
      this.previousVersions = file.previousVersions;
      this.$refs.prevModal.show();
    },
    removeFile(file) {
      this.fileToDelete.id = file.id;
      this.fileToDelete.name = file.name;
      this.showModal();
    },
    showModal() {
      this.$refs.delModal.show();
    },
    async hideModalOnProceed() {
      this.loading = true;
      await this.deleteFile(this.fileToDelete.id);
      await this.setFilesList();
      this.loading = false;
      this.$refs.delModal.hide();
    },
    hideModalOnCancel() {
      this.$refs.delModal.hide();
    },
    hidePreviousVersionsModal() {
      this.previousVersions = [];
      this.$refs.prevModal.hide();
    },
    download(fileId, fileName) {
      this.downloadFile(fileId).then((response) => {
        const fileType = response.headers.contenttype;
        const blob = new Blob([response.data], { type: `${fileType}` });
        let link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
      });
    },
    addDirectory() {
      this.$refs.addDirModal.show();
    },
    hideAddDirModalOnCancel() {
      this.dirName = "";
      this.$refs.addDirModal.hide();
    },
    async hideAddDirModalOnProceed() {
      this.loading = true;
      await this.addDir(this.dirName);
      await this.setDirectoriesList();
      this.loading = false;
      this.dirName = "";
      this.$refs.addDirModal.hide();
    },
    async routerPushToFolder(directoryId) {
      this.$router.push({ name: `home.index`, params: { dirId: directoryId } });
      this.loading = true;
      await this.setFilesList(this.$route.params.dirId);
      await this.setDirectoriesList(this.$route.params.dirId);
      this.loading = false;
    },
    async goBack() {
      this.$router.go(-1);
      this.loading = true;
      await this.setFilesList(this.$route.params.dirId);
      await this.setDirectoriesList(this.$route.params.dirId);
      await this.setFilesList(this.$route.params.dirId);
      this.loading = false;
    },
  },
  mounted() {
    this.loading = true;
    this.setFilesList(this.$route.params.dirId);
    this.setDirectoriesList(this.$route.params.dirId);
    this.loading = false;
  },
  components: {
    FileTile,
    LoadSpinner,
  },
};
</script>
<style scoped>
.button {
  border-radius: 5px;
  background-color: white;
  border-color: #22659f;
  color: #22659f;
  width: 100px;
  font-size: 16px;
}
.button-wide {
  border-radius: 5px;
  background-color: white;
  border-color: #22659f;
  color: #22659f;
  width: 150px;
  font-size: 16px;
}
.flexxin {
  display: flex;
  flex-wrap: wrap;
}
.no-files {
  color: grey;
  font-size: 24px;
}
/* .trash {
  margin-left: -5px;
} */
.trash:hover {
  cursor: pointer;
  color: red;
}
/* .versions {
  margin-left: -15px;
} */
.versions:hover {
  cursor: pointer;
  color: green;
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

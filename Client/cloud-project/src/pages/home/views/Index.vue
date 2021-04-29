<template>
  <div>
    <div class="mb-4">
      <input type="file" ref="input" />
      <button class="button" @click="submitFile">Submit</button>
    </div>
    <div class="flexxin">
      <div v-for="file in getFilesList" v-bind:key="file.id" class="mr-4 mb-4">
        <div class="trash" @click="removeFile(file)">
          <i class="fas fa-trash-alt trash"></i>
        </div>
        <FileTile
          :fileId="file.id"
          :fileType="file.type"
          :fileName="file.name"
        />
      </div>
      <div v-if="this.filesList.length === 0" class="no-files">
        Nie masz żadnych plików
      </div>
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
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";
import { mapFields } from "vuex-map-fields";

import FileTile from "../components/FileTile";

const STORE = "HomeStore";

export default {
  name: "home",
  data() {
    return {
      file: null,
      fileToDelete: {
        name: "",
        id: "",
      },
    };
  },
  computed: {
    ...mapFields(STORE, ["filesList", "file.source", "file.name", "file.type"]),
    ...mapGetters(STORE, ["getFilesList"]),
  },
  methods: {
    ...mapActions(STORE, ["setFilesList", "uploadFile", "deleteFile"]),
    submitFile() {
      let newFile = this.$refs.input.files[0];
      this.name = newFile.name;
      this.type = newFile.type;
      this.convertToBase64(newFile);
    },
    convertToBase64(file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      let _vm = this;
      reader.onload = function() {
        _vm.upload(reader.result);
      };
    },
    upload(fileSource) {
      this.source = fileSource;
      this.uploadFile();
    },
    removeFile(file) {
      this.fileToDelete.id = file.id;
      this.fileToDelete.name = file.name;
      this.showModal();
    },
    showModal() {
      this.$refs.delModal.show();
    },
    hideModalOnProceed() {
      this.deleteFile(this.fileToDelete.id);
      this.setFilesList();
      this.$refs.delModal.hide();
    },
    hideModalOnCancel() {
      this.$refs.delModal.hide();
    },
  },
  mounted() {
    this.setFilesList();
  },
  components: {
    FileTile,
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
.flexxin {
  display: flex;
  flex-wrap: wrap;
}
.no-files {
  color: grey;
  font-size: 24px;
}
.trash {
  margin-left: -5px;
}
.trash:hover {
  cursor: pointer;
  color: red;
}
</style>

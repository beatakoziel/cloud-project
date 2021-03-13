<template>
  <div>
    <div class="mb-4">
      <button type="button" class="button">Dodaj</button>
    </div>
    <div class="flexxin">
      <div v-for="file in getFilesList" v-bind:key="file.id" class="mr-4 mb-4">
        <FileTile :fileType="file.type" :fileName="file.name" />
      </div>
    </div>
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";

import FileTile from "../components/FileTile";

const STORE = "HomeStore";

export default {
  name: "home",
  computed: {
    ...mapGetters(STORE, ["getFilesList"]),
  },
  methods: {
    ...mapActions(STORE, ["setFilesList"]),
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
  width: 120px;
  font-size: 24px;
}

.flexxin {
  display: flex;
  flex-wrap: wrap;
}
</style>

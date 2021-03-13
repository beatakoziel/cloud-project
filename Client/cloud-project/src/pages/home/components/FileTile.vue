<template>
  <div>
    <div class="tile">
      <div class="icon d-flex justify-content-center">
        <div v-if="fileType === 'pdf'">
          <i class="fas fa-file-pdf"></i>
        </div>
        <div v-else-if="fileType === 'jpg'">
          <i class="fas fa-file-image"></i>
        </div>
        <div v-else-if="fileType === 'mp3'">
          <i class="fas fa-file-audio"></i>
        </div>
        <div v-else>
          <i class="fas fa-file"></i>
        </div>
      </div>
      <div class="name d-flex justify-content-center pl-2 pr-2">
        {{ `${this.fileName}.${this.fileType}` }}
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: {
    fileName: {
      type: String,
      required: true,
    },
    fileType: {
      type: String,
      required: true,
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
}
</style>

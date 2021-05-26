<template>
  <div>
    <div class="tile">
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
.tile:hover{
  box-shadow: 0 0 11px rgba(33,33,33,.2); 
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
  white-space: wrap;
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

<template>
  <div id="app">
    <Header />
    <div class="content">
      <router-view />
    </div>
  </div>
</template>

<script>
import Header from "./components/Header";

export default {
  name: "App",
  components: {
    Header,
  },
};
</script>

<style>
.content {
  width: auto;
  margin-top: 80px;
  margin-left: 160px;
  margin-right: 160px;
}
</style>

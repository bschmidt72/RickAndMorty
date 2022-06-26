<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
</script>

<template>
  <div class="page">
    <div class="header">
      <img class="logo" src="./logo.png" alt="Rick and Morty" />
      <h3>Character Search</h3>
    </div>
    <div class="divider"></div>
    <RouterView />
  </div>
</template>

<style>
body {
  width: 100vw;
  color: rgba(255, 255, 255, 0.55);
  background-color: #5052b5;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 13pt;
  font-weight: normal;
  line-height: 1.75;
  margin: 0;
  overflow-x: hidden;
}

.page {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 100%;
  min-height: 100vh;
}
.header {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: flex-end;
  justify-content: center;
  max-height: 160px;
  min-height: 120px;
  max-width: 100%;
  background-color: #5052b5;
}
.header h3 {
  color: rgb(255, 255, 255, 0.7);
  margin-left: 1em;
}
.divider {
  width: 100vw;
  border-bottom: solid 4px #b74e91;
}
.logo {
  max-width: 100%;
  max-height: 120px;
}
p {
  margin: 0 0 2em 0;
}

h1,
h2,
h3,
h4,
h5,
h6 {
  color: #ffffff;
  font-weight: bold;
  line-height: 1.5;
  margin: 0 0 0.5em 0;
  word-break: break-word;
}

h1 {
  font-size: 2.75em;
}
h2 {
  font-size: 1.75em;
}

h3 {
  font-size: 1.1em;
}

h4 {
  font-size: 1em;
}

h5 {
  font-size: 0.8em;
}

ol,
ul {
  list-style: none;
}

nav {
  width: 100%;
  font-size: 16px;
  text-align: center;
  margin-top: 2rem;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}

.styled-scrollbars {
  /* Foreground, Background */
  scrollbar-color: white #b74e91;
}
.styled-scrollbars::-webkit-scrollbar {
  width: 10px; /* Mostly for vertical scrollbars */
  height: 10px; /* Mostly for horizontal scrollbars */
}
.styled-scrollbars::-webkit-scrollbar-thumb {
  /* Foreground */
  background: white;
}
.styled-scrollbars::-webkit-scrollbar-track {
  /* Background */
  background: #b74e91;
}

@media screen and (max-width: 800px) {
  html,
  body {
    min-width: 100vw;
    font-size: 11pt;
    font-weight: normal;
    line-height: 1.25;
  }
  .header {
    max-height: 120px;
    min-height: 80px;
    max-width: 100%;
    background-color: #5052b5;
  }
  .header h3 {
    color: rgb(255, 255, 255, 0.7);
    margin-left: 1em;
  }
  .divider {
    width: 100vw;
    border-bottom: solid 4px #b74e91;
  }
  .logo {
    max-width: 100%;
    max-height: 80px;
  }
}
</style>

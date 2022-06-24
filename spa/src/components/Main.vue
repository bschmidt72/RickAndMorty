<template>
  <div class="container">
    <section id="sidebar">
      <div class="inner">
        <CharacterSearch @characterSelected="loadCharacter" @searchResults="displayResults"/>
      </div>
    </section>
    <div v-if="selectedCharacter" class="wrapper">
      <CharacterDisplay :character="selectedCharacter"/>
    </div>
    <div v-if="searchResults" class="wrapper">
      <div>
        <h3>Search Results</h3>
        <ul class="styled-scrollbars">
          <li v-for="character of searchResults" @click="loadCharacter(character)">{{character.name}}</li>
        </ul>
      </div>
    </div>
    <div v-if="!selectedCharacter && !searchResults" class="wrapper">
     <p>Please search for a character</p> 
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import type {Ref} from "vue";
import { RickAndMortyService } from "@/services/RickAndMortyService";
import type {Character} from "@/model/Character";
import CharacterSearch from "@/components/CharacterSearch.vue";
import CharacterDisplay from "@/components/CharacterDisplay.vue";
import type {CharacterInfo} from "@/model/CharacterInfo";

const service = new RickAndMortyService();
const selectedCharacter: Ref<Character | null> = ref(null);
const searchResults: Ref<CharacterInfo[] | null> = ref(null);

function loadCharacter(characterInfo: CharacterInfo) {
  service.getCharacter(characterInfo.id).then(result => selectedCharacter.value = result);
  searchResults.value = null;
}

function displayResults(results: CharacterInfo[]) {
  searchResults.value = results;
  selectedCharacter.value = null;
}
</script>

<style scoped>
.container {
  display: flex;
  flex-direction: row;
  flex: 1 1 auto;
  max-width: 100%;
  justify-content: center;
}
#sidebar {
  display: flex;
  flex-direction: column;
  flex: 1 1 25%;
  padding: 2.5em 2.5em 0.5em 2.5em ;
  background: #312450;
  cursor: default;
  min-height: 100vh;
  overflow-x: hidden;
  overflow-y: auto;
  min-width: 18em;
  z-index: 10000;
}

#sidebar > .inner {
  display: flex;
  flex: 1 1 auto;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  transition: opacity 1s ease;
  opacity: 1;
  width: 100%;
}

.wrapper {
  display: flex;
  flex-direction: column;
  flex: 1 1 75%;
  background-color: #5052b5;
}

.wrapper > * {
  padding: 2.5em 2.5em 0.5em 2.5em;
  max-width: 100%;
}

::placeholder { 
  color: red;
  opacity: 1; /* Firefox */
}

ul {
  max-height: 620px;
  overflow-y: auto;
  border: solid 1px #b74e91;
  max-width: calc(100% - 5em);
}

li {
  margin: 0 1em;
  flex: 1 1 100%;
  list-style: none;
  text-align: left;
  padding: 4px 2px;
  cursor: pointer;
  max-width: calc(100% - 48px);
  word-break: break-all;
}
li:hover {
  background-color: #b74e91;
  opacity: 0.5;
  color: white;
}

@media screen and (max-width: 800px) {
  .container {
    display: flex;
    flex-direction: column;
    flex: 1 1 auto;
  }
  #sidebar {
    max-width: calc(100% - 1em);
    flex: 1 1 auto;
    width: 100%;
    min-height: 50px;
    max-height: 200px;
    overflow-x: hidden;
    overflow-y: auto;
    min-width: auto;
    z-index: 10000;
    padding: 0.5em 0.5em 1.5em 0.5em ;
  }
  .wrapper {
    display: flex;
    flex-direction: column;
    flex: 1 1 auto;
    background-color: #5052b5;
  }

  .wrapper > * {
    padding: 0.5em 0.5em 0.5em 0.5em;
    max-width: calc(100% - 1em);
    width: calc(100% - 1em);
  }
  ul {
    max-height: 620px;
    overflow-y: auto;
    border: solid 1px #b74e91;
  }

}
</style>
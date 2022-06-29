const expectedResults = [
    '26 Years Old Morty',
    'Jerry 5-126',
    'Morty K-22',
    'Rick J-22',
    'Rick K-22'
];
describe('Rick and Morty E2E Tests', () => {
    beforeEach(() => {
        cy.visit('/')
    })

    it('verifies suggestions', () => {
        cy.get('#search-box')
            .type('2').should('have.value', '2')
        cy.get('.suggestions', { timeout: 10000 }).should('be.visible');
        cy.get('li.suggestion span').each((element, index, list) => {
            expect(element.text()).to.be.equal(expectedResults[index]);
        });
            
    })

    it('verifies search results', () => {
        cy.get('#search-box')
            .type('2').should('have.value', '2')
        cy.get('#search-box')
            .type('{enter}')
        cy.get('.search-result', { timeout: 10000 }).should('be.visible');
        cy.get('.search-result').each((element, index, list) => {
            expect(element.text()).to.be.equal(expectedResults[index]);
        });
    })

    it('verifies result after suggestion click', () => {
        cy.get('#search-box')
            .type('2').should('have.value', '2')
        cy.get('.suggestions', { timeout: 10000 }).should('be.visible');
        cy.get('li.suggestion span').each((element, index, list) => {
            expect(element.text()).to.be.equal(expectedResults[index]);
        });
        cy.get('li.suggestion').eq(2).click()
        cy.get('.character-page', { timeout: 10000 }).should('be.visible');
        cy.get('.character-page #character-name').should('have.text', 'Morty K-22')
        cy.get('.character-page #character-location').should('have.text', 'Location: Earth (Replacement Dimension)')
        cy.get('.character-page #character-status').should('have.text', 'Status: Alive')
    })

    it('verifies result after search result click', () => {
        cy.get('#search-box')
            .type('2').should('have.value', '2')
        cy.get('#search-box')
            .type('{enter}')
        cy.get('.search-result', { timeout: 10000 }).should('be.visible');
        cy.get('.search-result').each((element, index, list) => {
            expect(element.text()).to.be.equal(expectedResults[index]);
        });
        cy.get('.search-result').eq(2).click();
        cy.get('.character-page', { timeout: 10000 }).should('be.visible');
        cy.get('.character-page #character-name').should('have.text', 'Morty K-22')
        cy.get('.character-page #character-location').should('have.text', 'Location: Earth (Replacement Dimension)')
        cy.get('.character-page #character-status').should('have.text', 'Status: Alive')
    })

});
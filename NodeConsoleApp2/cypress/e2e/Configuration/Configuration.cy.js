// =====================================================
// VEHICLE CONFIGURATION CYPRESS TEST SUITE
// =====================================================

describe('Vehicle Configuration Management System', () => {

    beforeEach(() => {

        cy.login()

        Cypress.on('uncaught:exception', (err) => {

            // returning false here prevents Cypress from

            // failing the test

            console.log('Cypress detected uncaught exception: ', err);

            return false;

        });

    })
   
    it('test 1: Add Shift Configuration', () => {
        cy.get('#bharat').click();
    })

    beforeEach(() => {
        // Visit the configuration page before each test
        cy.visit('/Configuration1/ExtendedVehicleConfiguration');

        // Wait for page to load
        cy.get('[data-bs-toggle="tab"]').should('be.visible');

        // Wait for DataTables to initialize
        cy.wait(2000);
    });

    // =====================================================
    // TAB 1: CAR COLORS TESTS
    // =====================================================

    describe('Car Colors Management', () => {
        beforeEach(() => {
            // Click on Car Colors tab
            cy.get('#color-tab').click();
            cy.get('#colorTab').should('have.class', 'show active');
        });

        it('should display car colors table', () => {
            cy.get('#CarColorTable').should('be.visible');
            cy.get('#CarColorTable thead th').should('have.length', 12);
        });

        it('should open add car color modal', () => {
            cy.contains('Add Car Color').click();
            cy.get('#carColorModal').should('be.visible');
            cy.get('#colorNameInput').should('be.visible');
        });

        it('should add a new car color', () => {
            cy.contains('Add Car Color').click();

            // Fill form
            cy.get('#colorNameInput').type('Test Red');
            cy.get('#colorDisplayNameInput').type('Bright Red');
            cy.get('#colorHexCodeInput').type('#FF0000');
            cy.get('#colorFamilyInput').type('Red');
            cy.get('#isSolidPaintInput').check();
            cy.get('#popularityRankInput').type('1');
            cy.get('#resaleValueImpactInput').select('High');

            // Save
            cy.get('#btnCarColorSave').click();

            // Verify success (assuming alert or table update)
            cy.get('#CarColorTable tbody tr').should('contain', 'Test Red');
        });

        it('should validate required fields', () => {
            cy.contains('Add Car Color').click();
            cy.get('#btnCarColorSave').click();

            // Check for validation
            cy.get('#colorNameInput').should('have.class', 'is-invalid');
        });

        it('should edit existing car color', () => {
            // Assuming there's data in table
            cy.get('#CarColorTable tbody tr').first().within(() => {
                cy.get('.btn-edit').click();
            });

            cy.get('#carColorModal').should('be.visible');
            cy.get('#colorNameInput').should('not.be.empty');

            // Modify and save
            cy.get('#colorDisplayNameInput').clear().type('Modified Color');
            cy.get('#btnCarColorSave').click();
        });

        it('should delete car color with confirmation', () => {
            cy.get('#CarColorTable tbody tr').first().within(() => {
                cy.get('.btn-delete').click();
            });

            // Handle confirmation dialog
            cy.on('window:confirm', (str) => {
                expect(str).to.contain('Are you sure');
                return true;
            });
        });
    });

    // =====================================================
    // TAB 2: CAR CONDITIONS TESTS
    // =====================================================

    describe('Car Conditions Management', () =>
    {
        beforeEach(() => {
            cy.get('#condition-tab').click();
            cy.get('#conditionTab').should('have.class', 'show active');
        });

        it('should display car conditions table', () => {
            cy.get('#CarConditionTable').should('be.visible');
            cy.get('#CarConditionTable thead th').should('have.length', 10);
        });

        it('should add new car condition', () => {
            cy.contains('Add Car Condition').click();

            cy.get('#conditionNameInput').type('Excellent');
            cy.get('#conditionDescriptionInput').type('Like new condition');
            cy.get('#conditionPercentageInput').type('95');
            cy.get('#expectedPriceReductionInput').type('5');
            cy.get('#typicalIssuesInput').type('Minor wear');
            cy.get('#recommendedForInput').type('Premium buyers');
            cy.get('#conditionSortOrderInput').type('1');

            cy.get('#btnCarConditionSave').click();

            cy.get('#CarConditionTable tbody tr').should('contain', 'Excellent');
        });

        it('should validate percentage fields', () => {
            cy.contains('Add Car Condition').click();

            cy.get('#conditionPercentageInput').type('150'); // Invalid percentage
            cy.get('#expectedPriceReductionInput').type('-5'); // Invalid negative

            // Should handle validation
            cy.get('#conditionPercentageInput').should('have.attr', 'max', '100');
        });
    });

    // =====================================================
    // TAB 3: STATES TESTS
    // =====================================================

    describe('States Management', () => {
        beforeEach(() => {
            cy.get('#state-tab').click();
            cy.get('#stateTab').should('have.class', 'show active');
        });

        it('should display states table', () => {
            cy.get('#StateTable').should('be.visible');
        });

        it('should add new state', () => {
            cy.contains('Add State').click();

            cy.get('#stateNameInput').type('Test State');
            cy.get('#stateCodeInput').type('TS');
            cy.get('#stateRegionInput').type('North');
            cy.get('#countryNameInput').should('have.value', 'India');
            cy.get('#statePinCodePrefixInput').type('123');
            cy.get('#popularForCarsInput').check();

            cy.get('#btnStateSave').click();

            cy.get('#StateTable tbody tr').should('contain', 'Test State');
        });

        it('should prevent duplicate state names', () => {
            cy.contains('Add State').click();
            cy.get('#stateNameInput').type('Maharashtra'); // Assuming this exists

            // Should show duplicate message
            cy.get('#stateNameMessage').should('contain', 'already exists');
            cy.get('#btnStateSave').should('be.disabled');
        });
    });

    // =====================================================
    // TAB 4: CITIES TESTS
    // =====================================================

    describe('Cities Management', () => {
        beforeEach(() => {
            cy.get('#city-tab').click();
            cy.get('#cityTab').should('have.class', 'show active');
        });

        it('should display cities table', () => {
            cy.get('#CityTable').should('be.visible');
        });

        it('should add new city with state selection', () => {
            cy.contains('Add City').click();

            cy.get('#cityNameInput').type('Test City');
            cy.get('#stateIdInput').select('Maharashtra'); // Assuming state exists
            cy.get('#cityTypeInput').select('Tier 1');
            cy.get('#cityPopulationInput').type('1000000');
            cy.get('#typicalCarDemandInput').select('High');
            cy.get('#hasGoodCarMarketInput').check();

            cy.get('#btnCitySave').click();

            cy.get('#CityTable tbody tr').should('contain', 'Test City');
        });

        it('should require state selection', () => {
            cy.contains('Add City').click();

            cy.get('#cityNameInput').type('Test City');
            // Don't select state
            cy.get('#btnCitySave').click();

            cy.get('#stateIdInput').should('have.class', 'is-invalid');
        });
    });

    // =====================================================
    // TAB 5: ENGINE SPECIFICATIONS TESTS
    // =====================================================

    describe('Engine Specifications Management', () => {
        beforeEach(() => {
            cy.get('#engine-tab').click();
            cy.get('#engineTab').should('have.class', 'show active');
        });

        it('should display engine specs table', () => {
            cy.get('#EngineSpecTable').should('be.visible');
        });

        it('should add new engine specification', () => {
            cy.contains('Add Engine Spec').click();

            cy.get('#engineTypeInput').type('V6 Petrol');
            cy.get('#engineCylindersInput').type('6');
            cy.get('#engineValveConfigurationInput').select('DOHC');
            cy.get('#turbochargedEngineInput').check();
            cy.get('#engineDescriptionInput').type('High performance V6 engine');
            cy.get('#performanceRatingInput').select('Excellent');

            cy.get('#btnEngineSpecSave').click();

            cy.get('#EngineSpecTable tbody tr').should('contain', 'V6 Petrol');
        });

        it('should handle engine configuration checkboxes', () => {
            cy.contains('Add Engine Spec').click();

            cy.get('#engineTypeInput').type('Hybrid Engine');
            cy.get('#hybridSystemInput').check();
            cy.get('#naturallyAspiratedEngineInput').check();

            // Verify checkboxes are checked
            cy.get('#hybridSystemInput').should('be.checked');
            cy.get('#naturallyAspiratedEngineInput').should('be.checked');
        });
    });

    // =====================================================
    // TAB 6: INSURANCE PROVIDERS TESTS
    // =====================================================

    describe('Insurance Providers Management', () => {
        beforeEach(() => {
            cy.get('#insurance-tab').click();
            cy.get('#insuranceTab').should('have.class', 'show active');
        });

        it('should display insurance providers table', () => {
            cy.get('#InsuranceProviderTable').should('be.visible');
        });

        it('should add new insurance provider', () => {
            cy.contains('Add Insurance Provider').click();

            cy.get('#providerNameInput').type('Test Insurance Co.');
            cy.get('#providerTypeInput').select('Private');
            cy.get('#providerRatingInput').type('4.5');
            cy.get('#claimSettlementRatioInput').type('98.5');
            cy.get('#customerServiceRatingInput').select('Excellent');

            cy.get('#btnInsuranceProviderSave').click();

            cy.get('#InsuranceProviderTable tbody tr').should('contain', 'Test Insurance Co.');
        });

        it('should validate rating ranges', () => {
            cy.contains('Add Insurance Provider').click();

            cy.get('#providerRatingInput').type('6'); // Invalid rating > 5
            cy.get('#claimSettlementRatioInput').type('150'); // Invalid percentage > 100

            // Should enforce validation
            cy.get('#providerRatingInput').should('have.attr', 'max', '5');
            cy.get('#claimSettlementRatioInput').should('have.attr', 'max', '100');
        });
    });

    // =====================================================
    // TAB 7: CAR FEATURES TESTS
    // =====================================================

    describe('Car Features Management', () => {
        beforeEach(() => {
            cy.get('#feature-tab').click();
            cy.get('#featureTab').should('have.class', 'show active');
        });

        it('should display car features table', () => {
            cy.get('#CarFeatureTable').should('be.visible');
        });

        it('should add new car feature', () => {
            cy.contains('Add Car Feature').click();

            cy.get('#featureNameInput').type('Adaptive Cruise Control');
            cy.get('#featureDisplayNameInput').type('ACC');
            cy.get('#featureCategoryInput').select('Safety');
            cy.get('#featureSubCategoryInput').type('Driver Assistance');
            cy.get('#featureDescriptionInput').type('Maintains safe distance automatically');
            cy.get('#featureImportanceLevelInput').select('High');
            cy.get('#typicalFoundInInput').type('Premium Cars');
            cy.get('#affectsResaleValueInput').check();
            cy.get('#featureIconInput').type('fa-car');

            cy.get('#btnCarFeatureSave').click();

            cy.get('#CarFeatureTable tbody tr').should('contain', 'Adaptive Cruise Control');
        });

        it('should handle feature importance and category', () => {
            cy.contains('Add Car Feature').click();

            cy.get('#featureCategoryInput').select('Entertainment');
            cy.get('#featureImportanceLevelInput').select('Critical');
            cy.get('#isStandardFeatureInput').check();

            // Verify selections
            cy.get('#featureCategoryInput').should('have.value', 'Entertainment');
            cy.get('#featureImportanceLevelInput').should('have.value', 'Critical');
            cy.get('#isStandardFeatureInput').should('be.checked');
        });
    });

    // =====================================================
    // TAB 8: RTO CODES TESTS
    // =====================================================

    describe('RTO Codes Management', () => {
        beforeEach(() => {
            cy.get('#rto-tab').click();
            cy.get('#rtoTab').should('have.class', 'show active');
        });

        it('should display RTO codes table', () => {
            cy.get('#RtoCodeTable').should('be.visible');
        });

        it('should add new RTO code', () => {
            cy.contains('Add RTO Code').click();

            cy.get('#rtoCodeInput').type('MH01');
            cy.get('#rtoNameInput').type('Mumbai Central RTO');
            cy.get('#rtoStateIdInput').select('Maharashtra');

            // Wait for cities to load
            cy.wait(1000);
            cy.get('#rtoCityIdInput').select('Mumbai');

            cy.get('#rtoAddressInput').type('123 RTO Building, Mumbai');
            cy.get('#rtoContactNumberInput').type('022-12345678');

            cy.get('#btnRtoCodeSave').click();

            cy.get('#RtoCodeTable tbody tr').should('contain', 'MH01');
        });

        it('should handle state-city dependency', () => {
            cy.contains('Add RTO Code').click();

            cy.get('#rtoStateIdInput').select('Karnataka');

            // Cities should update based on state selection
            cy.wait(1000);
            cy.get('#rtoCityIdInput option').should('contain', 'Bangalore');
        });

        it('should validate required RTO fields', () => {
            cy.contains('Add RTO Code').click();
            cy.get('#btnRtoCodeSave').click();

            // Should validate required fields
            cy.get('#rtoCodeInput').should('have.class', 'is-invalid');
            cy.get('#rtoStateIdInput').should('have.class', 'is-invalid');
        });
    });

    // =====================================================
    // CROSS-TAB FUNCTIONALITY TESTS
    // =====================================================

    describe('Cross-tab Functionality', () => {
        it('should navigate between all tabs', () => {
            const tabs = [
                { id: '#color-tab', content: '#colorTab' },
                { id: '#condition-tab', content: '#conditionTab' },
                { id: '#state-tab', content: '#stateTab' },
                { id: '#city-tab', content: '#cityTab' },
                { id: '#engine-tab', content: '#engineTab' },
                { id: '#insurance-tab', content: '#insuranceTab' },
                { id: '#feature-tab', content: '#featureTab' },
                { id: '#rto-tab', content: '#rtoTab' }
            ];

            tabs.forEach(tab => {
                cy.get(tab.id).click();
                cy.get(tab.content).should('have.class', 'show active');
            });
        });

        it('should maintain data integrity across tabs', () => {
            // Add data in one tab
            cy.get('#state-tab').click();
            cy.contains('Add State').click();
            cy.get('#stateNameInput').type('Test State for Cities');
            cy.get('#btnStateSave').click();

            // Navigate to cities and check if state is available
            cy.get('#city-tab').click();
            cy.contains('Add City').click();
            cy.get('#stateIdInput').should('contain.text', 'Test State for Cities');
        });
    });

    // =====================================================
    // ERROR HANDLING TESTS
    // =====================================================

    describe('Error Handling', () => {
        it('should handle server errors gracefully', () => {
            // Mock server error
            cy.intercept('POST', '/Configuration1/AddOrEditCarColor', {
                statusCode: 500,
                body: { error: 'Internal Server Error' }
            }).as('serverError');

            cy.get('#color-tab').click();
            cy.contains('Add Car Color').click();
            cy.get('#colorNameInput').type('Test Color');
            cy.get('#btnCarColorSave').click();

            cy.wait('@serverError');
            // Should handle error appropriately
        });

        it('should handle network timeouts', () => {
            cy.intercept('GET', '/Configuration1/GetCarColors', {
                delayMs: 30000 // 30 second delay to simulate timeout
            }).as('timeout');

            cy.visit('/Configuration1/ExtendedVehicleConfiguration');

            // Should show loading state or error message
        });
    });

    // =====================================================
    // PERFORMANCE TESTS
    // =====================================================

    describe('Performance Tests', () => {
        it('should load tables within acceptable time', () => {
            const startTime = Date.now();

            cy.get('#CarColorTable').should('be.visible');

            cy.then(() => {
                const loadTime = Date.now() - startTime;
                expect(loadTime).to.be.lessThan(5000); // 5 seconds max
            });
        });

        it('should handle large datasets efficiently', () => {
            // Test with large dataset (if available)
            cy.get('#CarColorTable tbody tr').should('have.length.greaterThan', 0);

            // Test search functionality
            cy.get('#CarColorTable_filter input').type('Red');
            cy.get('#CarColorTable tbody tr:visible').should('have.length.lessThan', 50);
        });
    });

    // =====================================================
    // ACCESSIBILITY TESTS
    // =====================================================

    describe('Accessibility Tests', () => {
        it('should be keyboard navigable', () => {
            cy.get('#color-tab').focus().type('{enter}');
            cy.get('#colorTab').should('have.class', 'show active');

            cy.get('body').tab(); // Use tab key
            cy.focused().should('contain', 'Add Car Color');
        });

        it('should have proper ARIA labels', () => {
            cy.get('[role="tablist"]').should('exist');
            cy.get('[role="tab"]').should('have.length', 8);
            cy.get('[role="tabpanel"]').should('exist');
        });

        it('should work with screen readers', () => {
            cy.get('#color-tab').should('have.attr', 'aria-selected');
            cy.get('#colorTab').should('have.attr', 'role', 'tabpanel');
        });
    });

    // =====================================================
    // DATA VALIDATION TESTS
    // =====================================================

    describe('Data Validation', () => {
        it('should validate email formats in contact fields', () => {
            // If any email fields exist
            cy.get('input[type="email"]').then($emails => {
                if ($emails.length > 0) {
                    cy.wrap($emails.first()).type('invalid-email');
                    cy.wrap($emails.first()).should('have.attr', 'validity.valid', 'false');
                }
            });
        });

        it('should validate phone number formats', () => {
            cy.get('#rto-tab').click();
            cy.contains('Add RTO Code').click();

            cy.get('#rtoContactNumberInput').type('invalid-phone');
            // Should show validation error or format correctly
        });

        it('should validate numeric ranges', () => {
            cy.get('#condition-tab').click();
            cy.contains('Add Car Condition').click();

            cy.get('#conditionPercentageInput').type('-10');
            cy.get('#conditionPercentageInput').should('have.attr', 'min', '0');
        });
    });
});

// =====================================================
// CUSTOM CYPRESS COMMANDS
// =====================================================

// Add custom commands for common operations
Cypress.Commands.add('addCarColor', (colorData) => {
    cy.get('#color-tab').click();
    cy.contains('Add Car Color').click();

    cy.get('#colorNameInput').type(colorData.name);
    if (colorData.displayName) cy.get('#colorDisplayNameInput').type(colorData.displayName);
    if (colorData.hexCode) cy.get('#colorHexCodeInput').type(colorData.hexCode);
    if (colorData.family) cy.get('#colorFamilyInput').type(colorData.family);

    cy.get('#btnCarColorSave').click();
});

Cypress.Commands.add('deleteTableRow', (tableId, rowText) => {
    cy.get(`#${tableId} tbody tr`).contains(rowText).parent().within(() => {
        cy.get('.btn-delete').click();
    });

    cy.on('window:confirm', () => true);
});

Cypress.Commands.add('editTableRow', (tableId, rowText) => {
    cy.get(`#${tableId} tbody tr`).contains(rowText).parent().within(() => {
        cy.get('.btn-edit').click();
    });
});

// =====================================================
// TEST DATA FIXTURES
// =====================================================

// You can create fixtures for test data
const testData = {
    carColors: [
        { name: 'Test Red', displayName: 'Bright Red', hexCode: '#FF0000', family: 'Red' },
        { name: 'Test Blue', displayName: 'Ocean Blue', hexCode: '#0000FF', family: 'Blue' }
    ],
    states: [
        { name: 'Test State', code: 'TS', region: 'North', pinPrefix: '123' }
    ],
    cities: [
        { name: 'Test City', type: 'Tier 1', population: '1000000', demand: 'High' }
    ]
};

// Export test data for use in other files
export { testData };
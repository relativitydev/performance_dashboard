Feature: InstallSQLAdminScript

@GoldenFlow @ScriptInstallPage
Scenario: Admin installs SQL Admin Scripts through Install Scripts page.
    Given that I am on the Reinstall Scripts page.
    And I enter the valid SQL credentials on the Reinstall Scripts page.
    When I run the SQL Script Install.
    Then I receive SQL Script Install success message.
Feature: NavigateToQosPage

@GoldenFlow @Navigation @QualityofServicePage
Scenario Outline: Admin accesses a Performance Dashboard page.
    When I navigate to the '<Target Page>' page.
    Then the page '<Result Page>' loads.

Examples:
    | Target Page        | Result Page        |
    | Quality of Service | Quality of Service |
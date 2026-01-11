// This JavaScript file adds enhanced functionality to the service listing page

document.addEventListener('DOMContentLoaded', function () {
    // Add hover effects for service cards
    const serviceCards = document.querySelectorAll('.service-card');
    serviceCards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            card.classList.add('shadow-lg');
        });
        card.addEventListener('mouseleave', () => {
            card.classList.remove('shadow-lg');
        });
    });

    // Smooth scrolling for mobile when selecting categories
    const categoryLinks = document.querySelectorAll('.category-list a');
    categoryLinks.forEach(link => {
        link.addEventListener('click', function () {
            // On mobile, scroll to the services section
            if (window.innerWidth < 768) {
                const servicesSection = document.querySelector('.services-section');
                if (servicesSection) {
                    setTimeout(() => {
                        servicesSection.scrollIntoView({ behavior: 'smooth' });
                    }, 150);
                }
            }
        });
    });

    // Enhanced search experience
    const searchInput = document.querySelector('.service-search input');
    if (searchInput) {
        // Clear button functionality
        searchInput.addEventListener('input', function () {
            const clearButton = document.querySelector('.search-clear-btn');
            if (this.value && !clearButton) {
                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'search-clear-btn';
                btn.innerHTML = '&times;';
                btn.style.position = 'absolute';
                btn.style.right = '70px';
                btn.style.top = '50%';
                btn.style.transform = 'translateY(-50%)';
                btn.style.border = 'none';
                btn.style.background = 'transparent';
                btn.style.fontSize = '1.5rem';
                btn.style.color = '#6c757d';
                btn.style.cursor = 'pointer';

                btn.addEventListener('click', () => {
                    searchInput.value = '';
                    searchInput.focus();
                    btn.remove();
                });

                searchInput.parentNode.appendChild(btn);
            } else if (!this.value && clearButton) {
                clearButton.remove();
            }
        });

        // Add placeholder animation
        searchInput.addEventListener('focus', function () {
            this.setAttribute('placeholder', 'Type to search for services...');
        });

        searchInput.addEventListener('blur', function () {
            this.setAttribute('placeholder', 'Search services…');
        });
    }
});